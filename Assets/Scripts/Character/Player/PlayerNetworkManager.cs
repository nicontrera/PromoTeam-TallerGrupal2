using Unity.Netcode;
using Unity.Collections;
using UnityEngine;

namespace NC
{
    public class PlayerNetworkManager : CharacterNetworkManager
    {
        PlayerManager player;

        [Header("Live Calculated Stats (For Inspector)")]
        public int currentTotalAttack;
        public int currentTotalDefense;

        public int baseAttack = 10;
        public int baseDefense = 5;
        public ItemData equippedWeapon;
        public ItemData equippedArmor;
        public NetworkVariable<FixedString64Bytes> characterName = new NetworkVariable<FixedString64Bytes>("Character", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        [Header("Level and Range")]
        public NetworkVariable<int> playerLevel = new NetworkVariable<int>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        // public NetworkVariable<string> playerRange = new NetworkVariable<string>("knight", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<int> playerExp = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<int> expRequiredForNextLevel = new NetworkVariable<int>(50, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        

        [Header("Multiplayer Synced IDs")]
        // -1 represents an "Empty Slot" (naked / unarmed)
        public NetworkVariable<int> netWeaponID = new NetworkVariable<int>(-1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
        public NetworkVariable<int> netArmorID  = new NetworkVariable<int>(-1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);


        protected override void Awake() {
            base.Awake();

            player = GetComponent<PlayerManager>();
        }

        public override void OnNetworkSpawn()
        {
            // These hooks fire automatically for ALL players whenever the server changes an ID
            netWeaponID.OnValueChanged += (oldID, newID) => TranslateIDToGear(newID, true);
            netArmorID.OnValueChanged  += (oldID, newID) => TranslateIDToGear(newID, false);

            // Force the math to run Frame 1 so we start at 10, not 0!
            UpdateInspectorDisplay();
        }

        private void TranslateIDToGear(int itemID, bool isWeapon)
        {
            if (itemID == -1) // Player unequipped it
            {
                if (isWeapon) equippedWeapon = null;
                else equippedArmor = null;
                // return;
            }
            else
            {
                ItemData item = ItemDatabase.GetItemByID(itemID);
                if (isWeapon) equippedWeapon = item;
                else equippedArmor = item;
            }

            // // 2. NEW: Update the visual variables so you can see them in the Unity Inspector!
            // currentTotalAttack = GetTotalAttack();
            // currentTotalDefense = GetTotalDefense();
            UpdateInspectorDisplay();
        }

        private void UpdateInspectorDisplay()
        {
            currentTotalAttack = GetTotalAttack();
            currentTotalDefense = GetTotalDefense();
        }

        [ServerRpc]
        public void RequestEquipWeaponServerRpc(int itemID) => netWeaponID.Value = itemID;

        [ServerRpc]
        public void RequestEquipArmorServerRpc(int itemID)  => netArmorID.Value = itemID;

        public void SetNewMaxHealthValue(int oldVitality, int newVitality)
        {
            maxHealth.Value = player.playerStatsManager.CalculateHealthBasedOnVitalityLevel(newVitality);
            PlayerUIManager.instance.playerUIHudManager.SetMaxHealthValue(maxHealth.Value);
            currentHealth.Value = maxHealth.Value;
        }

        public void SetNewMaxStaminaValue(int oldEndurance, int newEndurance)
        {
            // THIS ALSO DOES UPDATES CURRENT TO MAX, LIKE WHEN LEVELING UP!
            maxStamina.Value = player.playerStatsManager.CalculateStaminaBasedOnEnduranceLevel(newEndurance);
            PlayerUIManager.instance.playerUIHudManager.SetMaxStaminaValue(maxStamina.Value);
            currentStamina.Value = maxStamina.Value;
        }

            public int GetTotalAttack()
        {
            int attack = baseAttack;
            if (equippedWeapon != null) attack += equippedWeapon.attackBonus;
            return attack;
        }

        public int GetTotalDefense()
        {
            int defense = baseDefense;
            if (equippedArmor != null) defense += equippedArmor.defenseBonus;
            return defense;
        }

        // Called by the Server via RPC when a player drinks a potion
        public void Heal(int amount)
        {
            if (!IsServer) return;
            currentHealth.Value = Mathf.Clamp(currentHealth.Value + amount, 0, GetMaxHP());
        }

        private int GetMaxHP()
        {
            int maxHp = 100; // Base Max HP
            if (equippedArmor != null) maxHp += equippedArmor.maxHpBonus;
            return maxHp;
        }



        // [ServerRpc]
        // public void NotifyAttackHitServerRpc(ulong targetNetworkObjectId, int damageToDeal, ulong sourceClientId)
        // {
        //     // SERVER AUTHORITY: Only the Server is legally allowed to alter a monster's HP
        //     if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(targetNetworkObjectId, out NetworkObject netObj))
        //     {
        //         Debug.Log($"TESTING 2ND IF");

        //         if (netObj.TryGetComponent(out EnemyManager enemy))
        //         {
        //             Debug.Log($"[SERVER] Player {sourceClientId} hit enemy {netObj.name} for {damageToDeal} damage!");
                    
        //             // Call the enemy's authoritative damage function
        //             enemy.TakeDamage(damageToDeal, sourceClientId);
                    
        //             // Check for death and reward EXP
        //             if (enemy.enemyNetworkManager.currentEnemyHealth.Value <= 0)
        //             {
        //                 player.CheckForLevelUpOk(30, sourceClientId);
        //             }
        //         }
        //     }
        // }


        [ServerRpc]
        public void NotifyAttackHitServerRpc(ulong targetNetworkObjectId, int damageToDeal, ulong sourceClientId)
        {
            bool existsInLedger = NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(targetNetworkObjectId, out NetworkObject netObj);
            
            // THE FIX: We ask the object for 'EnemyAI', not EnemyManager!
            EnemyAI goblin = null;
            bool hasEnemyScript = existsInLedger && netObj.TryGetComponent(out goblin);

            Debug.Log($"<color=yellow>[SERVER WIRETAP]</color> Client {sourceClientId} hit NetID [{targetNetworkObjectId}] | Found in Ledger? {existsInLedger} | Has EnemyAI? {hasEnemyScript}");

            if (existsInLedger && hasEnemyScript && goblin != null)
            {
                Debug.Log($"[SERVER] Authorized hit on {netObj.name} for {damageToDeal} damage.");
                
                // Trigger the Goblin's native damage RPC
                goblin.TakeDamageServerRpc(damageToDeal);

                // Check if the Goblin's NetworkVariable HP dropped to zero
                if (goblin.currentHP.Value <= 0)
                {
                    player.CheckForLevelUpOk(30, sourceClientId);
                }
            }
        }
    }
}