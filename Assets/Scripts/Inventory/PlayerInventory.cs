using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;
using NC;

public class PlayerInventory : NetworkBehaviour
{
    [Header("Ground Loot Setup")]
    public GameObject worldItemPrefab; // Drag your WorldItem.prefab here in the Inspector!
    public float pickupRadius = 2.5f;


    [Header("Debug Testing")]
    public ItemData testSword;
    public ItemData testPotion;


    public event System.Action OnInventoryChanged;
    public List<ItemStack> inventoryStacks = new List<ItemStack>();

// Add this NetworkBehaviour override so ONLY the local player grabs the UI:
    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            InventoryUI.Instance.ConnectPlayer(this);
        }
    }
    // A simple local list for the UI. For full multiplayer, you might sync this via a NetworkList<int>
    // public List<ItemData> items = new List<ItemData>(); 
    // public List<ItemStack> items = new List<ItemStack>(); 
    
    private PlayerNetworkManager stats;

    private void Awake()
    {
        stats = GetComponent<PlayerNetworkManager>();
    }

    // public void UseItem(ItemData item)
    // {
    //     if (item.type == ItemType.Consumable)
    //     {
    //         // Consumables must be processed on the server to update NetworkVariables
    //         UseConsumableServerRpc(item.itemID);
    //         items.Remove(item); // Remove from local inventory
    //     }
    //     else
    //     {
    //         // Equip logic can be local prediction first, then tell the server
    //         EquipItem(item);
    //     }
    // }

    // Update your UseItem method to trigger the event:
    // public void UseItem(ItemData item)
    // {
    //     if (item.type == ItemType.Consumable)
    //     {
    //         UseConsumableServerRpc(item.itemID);
    //         items.Remove(item); 
    //         OnInventoryChanged?.Invoke(); // TELLS THE UI TO UPDATE
    //     }
    //     else
    //     {
    //         EquipItem(item);
    //         // You might not want to remove equipment from the inventory list, 
    //         // but if you do, remove it here and call OnInventoryChanged.
    //     }
    // }

    // You will also need a method to pick up items later:
    // public void AddItem(ItemData item)
    // {
    //     items.Add(item);
    //     OnInventoryChanged?.Invoke(); // TELLS THE UI TO UPDATE
    // }

    [ServerRpc]
    private void UseConsumableServerRpc(int itemID)
    {
        // 1. Look up the item by ID in your global ItemDatabase
        ItemData item = ItemDatabase.GetItemByID(itemID); 
        
        if (item != null)
        {
            stats.Heal(item.restoreHpAmount);
            // Add MP restore logic here
        }
    }

    private void EquipItem(ItemData item)
    {
        // Update local equipment references. 
        // Because the stats are calculated dynamically in PlayerStats.cs, 
        // simply assigning the reference updates their Attack/Defense instantly.
        if (item.type == ItemType.Weapon)
        {
            stats.equippedWeapon = item;
            Debug.Log($"Equipped {item.itemName}. New Attack: {stats.GetTotalAttack()}");
        }
        else if (item.type == ItemType.Armor)
        {
            stats.equippedArmor = item;
            Debug.Log($"Equipped {item.itemName}. New Defense: {stats.GetTotalDefense()}");
        }
    }


    public void AddItem(ItemData itemToAdd, int amount = 1)
    {
        if (itemToAdd.isStackable)
        {
            // 1. Try to add to existing stacks first
            foreach (ItemStack stack in inventoryStacks)
            {
                if (stack.item.itemID == itemToAdd.itemID && stack.quantity < itemToAdd.maxStackSize)
                {
                    // Calculate how much room is left in this specific stack
                    int spaceLeft = itemToAdd.maxStackSize - stack.quantity;
                    int amountWeCanAdd = Mathf.Min(amount, spaceLeft);
                    
                    stack.quantity += amountWeCanAdd;
                    amount -= amountWeCanAdd;

                    // If we've added all the items, we are done
                    if (amount <= 0)
                    {
                        OnInventoryChanged?.Invoke();
                        return; 
                    }
                }
            }
        }

        // 2. If we still have items left (or it's an unstackable sword), create new slots
        while (amount > 0)
        {
            // Unstackable items force a quantity of 1 per slot
            int amountForNewStack = itemToAdd.isStackable ? Mathf.Min(amount, itemToAdd.maxStackSize) : 1;
            
            inventoryStacks.Add(new ItemStack(itemToAdd, amountForNewStack));
            amount -= amountForNewStack;
        }

        OnInventoryChanged?.Invoke();
    }

    // public void UseItem(ItemStack stack)
    // {
    //     if (stack.item.type == ItemType.Consumable)
    //     {
    //         UseConsumableServerRpc(stack.item.itemID);
            
    //         // Decrease quantity and remove the slot if it hits zero
    //         stack.quantity--;
    //         if (stack.quantity <= 0)
    //         {
    //             inventoryStacks.Remove(stack);
    //         }
            
    //         OnInventoryChanged?.Invoke();
    //     }
    //     else
    //     {
    //         EquipItem(stack.item);
    //         // Equipment usually stays in the inventory or moves to an "Equipped" slot, 
    //         // depending on your RPG design.
    //     }
    // }


    public void UseItem(ItemStack stack)
    {
        if (stack.item.type == ItemType.Consumable)
        {
            UseConsumableServerRpc(stack.item.itemID);
            stack.quantity--;
            if (stack.quantity <= 0) inventoryStacks.Remove(stack);
            OnInventoryChanged?.Invoke();
        }
        else if (stack.item.type == ItemType.Weapon)
        {
            // 1. If we already have a weapon equipped, throw it back in the bag first!
            // if (InventoryUI.Instance.weaponSlotUI.currentStack != null)
            if (InventoryUI.Instance.weaponSlotUI.currentStack?.item != null)
            {
                UnequipGear(ItemType.Weapon, InventoryUI.Instance.weaponSlotUI.currentStack);
            }

            // 2. Put the new weapon into the UI Box
            InventoryUI.Instance.weaponSlotUI.PutItemInSlot(stack);
            inventoryStacks.Remove(stack);

            // 3. Radio the server!
            stats.RequestEquipWeaponServerRpc(stack.item.itemID);

            OnInventoryChanged?.Invoke();
        }
        // (You can copy/paste the exact Weapon block above for 'ItemType.Armor')
        else if (stack.item.type == ItemType.Armor)
        {
            if (InventoryUI.Instance.armorSlotUI.currentStack?.item != null)
            {
                UnequipGear(ItemType.Armor, InventoryUI.Instance.armorSlotUI.currentStack);
            }

            InventoryUI.Instance.armorSlotUI.PutItemInSlot(stack);
            inventoryStacks.Remove(stack);

            stats.RequestEquipWeaponServerRpc(stack.item.itemID);

            OnInventoryChanged?.Invoke();
        }
    }


    // Called by EquipmentSlot.cs when the player clicks the equipped weapon to put it away
    public void UnequipGear(ItemType type, ItemStack gearToUnequip)
    {
        AddItem(gearToUnequip.item, 1); // Put it back in bag

        if (type == ItemType.Weapon)
        {
            stats.RequestEquipWeaponServerRpc(-1); // -1 tells server "Hands are empty"
        }
        else if (type == ItemType.Armor)
        {
            stats.RequestEquipArmorServerRpc(-1);
        }
    }


    private void Update()
    {
        // Only the local player should be able to trigger this cheat
        if (!IsOwner) return;

        // Use the 'T' key to spawn test items
        if (UnityEngine.InputSystem.Keyboard.current != null && 
            UnityEngine.InputSystem.Keyboard.current.tKey.wasPressedThisFrame)
        {
            if (testSword != null) AddItem(testSword, 1);
            if (testPotion != null) AddItem(testPotion, 5); 
            
            Debug.Log("Cheat activated: Added Test Items!");
        }


        // Press 'F' to pick up nearby ground items
        if (UnityEngine.InputSystem.Keyboard.current != null && 
            UnityEngine.InputSystem.Keyboard.current.fKey.wasPressedThisFrame)
        {
            ScanForNearbyLoot();
        }
    }




    private void ScanForNearbyLoot()
    {
        // Draw an invisible physics bubble around the player
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, pickupRadius);

        foreach (Collider hit in hitColliders)
        {
            if (hit.TryGetComponent(out WorldItemInstance lootInstance))
            {
                // Found one! Ask the server to grant it to us using its unique Network ID.
                ulong netObjID = lootInstance.GetComponent<NetworkObject>().NetworkObjectId;
                RequestPickupLootServerRpc(netObjID);
                break; // Only pick up one item per 'F' press
            }
        }
    }

    [ServerRpc]
    private void RequestPickupLootServerRpc(ulong targetNetworkObjectID)
    {
        // SERVER SECURITY CHECK: Look up the NetworkObject by its ID in the global registry
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(targetNetworkObjectID, out NetworkObject netObj))
        {
            WorldItemInstance loot = netObj.GetComponent<WorldItemInstance>();
            
            if (loot != null && loot.netItemID.Value != -1)
            {
                ItemData data = ItemDatabase.GetItemByID(loot.netItemID.Value);
                
                // 1. Give it to the player
                AddItem(data, loot.netQuantity.Value);

                // 2. Erase the ground object from the network
                loot.ClaimAndDestroy();
            }
        }
    }

    // Called by UI when a player decides to throw an item onto the floor
    [ServerRpc]
    public void DropItemServerRpc(int itemID, int quantity)
    {
        // Spawn it 1.5 meters in front of the player's chest
        Vector3 dropPosition = transform.position + (transform.forward * 1.5f) + (Vector3.up * 0.5f);
        
        GameObject newLoot = Instantiate(worldItemPrefab, dropPosition, Quaternion.identity);
        NetworkObject netObj = newLoot.GetComponent<NetworkObject>();
        
        netObj.Spawn(); // Tells Netcode: "Broadcast this object's birth to all connected PCs!"

        WorldItemInstance lootScript = newLoot.GetComponent<WorldItemInstance>();
        lootScript.netItemID.Value = itemID;
        lootScript.netQuantity.Value = quantity;
    }
}