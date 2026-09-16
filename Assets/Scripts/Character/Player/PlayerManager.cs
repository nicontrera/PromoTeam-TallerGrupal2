using UnityEngine;
using Unity.Netcode;

namespace NC
{
    public class PlayerManager : CharacterManager
    {
        public PlayerAnimatorManager playerAnimatorManager;
        public PlayerLocomotionManager playerLocomotionManager;

        public PlayerNetworkManager playerNetworkManager;
        public PlayerStatsManager playerStatsManager;

        [HideInInspector] public PlayerEquipmentManager playerEquipmentManager;
        [HideInInspector] public PlayerInventory playerInventory;
        public PlayerTargetingManager playerTargetingManager;

        public PlayerNetworkManager player;
        public NetworkObject playerGameObject;



        protected override void Awake()
        {
            base.Awake();
            // DO MORE STUFF, ONLY FOR THE PLAYER
            playerLocomotionManager = GetComponent<PlayerLocomotionManager>();
            playerAnimatorManager = GetComponent<PlayerAnimatorManager>();
            playerNetworkManager = GetComponent<PlayerNetworkManager>();
            playerStatsManager = GetComponent<PlayerStatsManager>();
            playerEquipmentManager = GetComponent<PlayerEquipmentManager>();
            playerInventory = GetComponent<PlayerInventory>();
            playerTargetingManager = GetComponent<PlayerTargetingManager>();
        }

        protected override void Update()
        {
            base.Update();

            // IF WE DO NOT OWN THIS GAMEOBJECT, WE DO NOT CONTROL OR EDIT IT
            if(!IsOwner)
            {
                return;
            }
            // HANDLE ALL OUR CHARACTER MOVEMENT
            playerLocomotionManager.HandleAllMovement();

            // REGEN STAMINA
            playerStatsManager.RegenerateStamina();
        }
        protected override void LateUpdate()
        {
            if (!IsOwner)
                return;
            
            base.LateUpdate();

            PlayerCamera.instance.HandleAllCameraActions();
        }
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            //  IF THIS IS THE PLAYER OBJECT OWNED BY THIS CLIENT
            if (IsOwner)
            {
                playerNetworkManager.endurance.Value = 12;
                playerNetworkManager.vitality.Value = 11;
                PlayerCamera.instance.player = this;
                PlayerInputManager.instance.player = this;
                WorldSaveGameManager.instance.player = this;

                // PlayerUIManager.instance.playerUIHudManager

                // UPDATES THE TOTAL AMOUNT OF HEALTH OR STAMINA WHEN THE STAT LINKED TO EITHER CHANGES (LIKE LEVELING UP)
                playerNetworkManager.vitality.OnValueChanged += playerNetworkManager.SetNewMaxHealthValue;
                playerNetworkManager.endurance.OnValueChanged += playerNetworkManager.SetNewMaxStaminaValue;

                // UPDATES UI BARS WHEN A STAT CHANGES (HEALTH OR STAMINA)
                playerNetworkManager.currentHealth.OnValueChanged += PlayerUIManager.instance.playerUIHudManager.SetNewHealthValue;
                playerNetworkManager.currentStamina.OnValueChanged += PlayerUIManager.instance.playerUIHudManager.SetNewStaminaValue;
                playerNetworkManager.currentStamina.OnValueChanged += playerStatsManager.ResetStaminaRegenTimer;

                // THIS WILL BE MOVED WHEN SAVING AND LOADING IS ADDED
                playerNetworkManager.maxHealth.Value = playerStatsManager.CalculateHealthBasedOnVitalityLevel(playerNetworkManager.vitality.Value);

                playerNetworkManager.maxStamina.Value = playerStatsManager.CalculateStaminaBasedOnEnduranceLevel(playerNetworkManager.endurance.Value);

                playerNetworkManager.currentHealth.Value = playerStatsManager.CalculateHealthBasedOnVitalityLevel(playerNetworkManager.vitality.Value);
                playerNetworkManager.currentStamina.Value = playerStatsManager.CalculateStaminaBasedOnEnduranceLevel(playerNetworkManager.endurance.Value);
                PlayerUIManager.instance.playerUIHudManager.SetMaxHealthValue(playerNetworkManager.maxHealth.Value);
                PlayerUIManager.instance.playerUIHudManager.SetMaxStaminaValue(playerNetworkManager.maxStamina.Value);
            }
        }
    
        public void SaveGameDataToCurrentCharacterData(ref CharacterSaveData currentCharacterData)
        {
            currentCharacterData.characterName = playerNetworkManager.characterName.Value.ToString();
            currentCharacterData.xPosition = transform.position.x;
            currentCharacterData.yPosition = transform.position.y;
            currentCharacterData.zPosition = transform.position.z;

            currentCharacterData.currentHealth = playerNetworkManager.currentHealth.Value;
            currentCharacterData.currentStamina = playerNetworkManager.currentStamina.Value;

            currentCharacterData.vitality = playerNetworkManager.vitality.Value; 
            currentCharacterData.endurance = playerNetworkManager.endurance.Value;

            // PROGRESSION
            currentCharacterData.playerLevel = playerNetworkManager.playerLevel.Value;
            currentCharacterData.playerExp = playerNetworkManager.playerExp.Value;
            currentCharacterData.expRequiredForNextLevel = playerNetworkManager.expRequiredForNextLevel.Value;

            // EQUIPPED GEAR
            currentCharacterData.equippedWeaponID = playerNetworkManager.netWeaponID.Value;
            currentCharacterData.equippedArmorID = playerNetworkManager.netArmorID.Value;

            // INVENTORY
            currentCharacterData.inventoryItems.Clear();
            if (playerInventory != null)
            {
                foreach (ItemStack stack in playerInventory.inventoryStacks)
                {
                    currentCharacterData.inventoryItems.Add(new InventoryItemSaveData
                    {
                        itemID = stack.item.itemID,
                        quantity = stack.quantity
                    });
                }
            }
        }

        public void LoadGameDataFromCurrentCharacterData(ref CharacterSaveData currentCharacterData)
        {
            playerNetworkManager.characterName.Value = currentCharacterData.characterName;
            Vector3 myPosition = new Vector3(currentCharacterData.xPosition, currentCharacterData.yPosition, currentCharacterData.zPosition);
            transform.position = myPosition;

            playerNetworkManager.vitality.Value = currentCharacterData.vitality; 
            playerNetworkManager.endurance.Value = currentCharacterData.endurance;

            // THIS WILL BE MOVED WHEN SAVING AND LOADING IS ADDED
            playerNetworkManager.maxHealth.Value = playerStatsManager.CalculateHealthBasedOnVitalityLevel(currentCharacterData.vitality);
            // playerNetworkManager.maxSHealth.Value = playerStatsManager.CalculateHealthBasedOnVitalityLevel(playerNetworkManager.vitality.Value); //ALTERNATIVE
            playerNetworkManager.maxStamina.Value = playerStatsManager.CalculateStaminaBasedOnEnduranceLevel(currentCharacterData.endurance);
            // playerNetworkManager.currentStamina.Value = playerStatsManager.CalculateStaminaBasedOnEnduranceLevel(currentCharacterData.endurance);
            playerNetworkManager.currentHealth.Value = currentCharacterData.currentHealth;
            playerNetworkManager.currentStamina.Value = currentCharacterData.currentStamina;
            PlayerUIManager.instance.playerUIHudManager.SetMaxStaminaValue(playerNetworkManager.maxStamina.Value);

            // PROGRESSION
            playerNetworkManager.playerLevel.Value = currentCharacterData.playerLevel;
            playerNetworkManager.playerExp.Value = currentCharacterData.playerExp;
            playerNetworkManager.expRequiredForNextLevel.Value = currentCharacterData.expRequiredForNextLevel;

            // INVENTORY - restored before gear, so the equip lookups below have items to find
            if (playerInventory != null)
            {
                playerInventory.inventoryStacks.Clear();
                foreach (InventoryItemSaveData savedItem in currentCharacterData.inventoryItems)
                {
                    ItemData item = ItemDatabase.GetItemByID(savedItem.itemID);
                    if (item != null)
                    {
                        playerInventory.inventoryStacks.Add(new ItemStack(item, savedItem.quantity));
                    }
                }
            }

            // EQUIPPED GEAR - routed through the existing ServerRpcs rather than writing netWeaponID/netArmorID
            // directly, since those NetworkVariables are Server-write-only and this method can also run on a
            // joining client loading their own save; the RPC call is safe from either host or client context,
            // and re-triggers TranslateIDToGear so the weapon/armor visuals and stats load correctly too.
            playerNetworkManager.RequestEquipWeaponServerRpc(currentCharacterData.equippedWeaponID);
            playerNetworkManager.RequestEquipArmorServerRpc(currentCharacterData.equippedArmorID);
        }

        public void CheckForLevelUpOk(int expGained, ulong playerId)
        {
            Debug.Log("Checking for lvl up!");
            // currentPlayerExp += expGained;

            playerGameObject = NetworkManager.Singleton.ConnectedClients[playerId].PlayerObject;

            player  = playerGameObject.GetComponent<PlayerNetworkManager>();

            player.playerExp.Value += expGained;

            if (player.playerExp.Value >= player.expRequiredForNextLevel.Value)
            {
                Debug.Log("LEVELING UP!");
                player.vitality.Value += 5;
                player.endurance.Value += 5;


                player.playerLevel.Value = player.playerLevel.Value + 1;
                player.playerExp.Value = player.playerExp.Value - player.expRequiredForNextLevel.Value;
                player.expRequiredForNextLevel.Value += 30; // Improve using a formula
                Debug.Log("Player lvl " + player.playerLevel.Value + ": Caballero Novato");
                Debug.Log("Current exp: " + player.playerExp.Value);
                Debug.Log("Exp required for lvlup: " + player.expRequiredForNextLevel.Value);
            }
            else
            {
                Debug.Log("Player lvl: " + player.playerLevel.Value);
                Debug.Log("Current exp: " + player.playerExp.Value);
                Debug.Log("Exp required for lvlup: " + player.expRequiredForNextLevel.Value);
            }
        }
    }
}
