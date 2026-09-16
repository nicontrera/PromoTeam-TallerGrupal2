using System.Collections.Generic;
using UnityEngine;

namespace NC
{
    // ItemData is a ScriptableObject reference and can't survive being written to a JSON file,
    // so inventory is saved as (itemID, quantity) pairs and rebuilt via ItemDatabase.GetItemByID on load.
    [System.Serializable]
    public class InventoryItemSaveData
    {
        public int itemID;
        public int quantity;
    }
    [System.Serializable]
    // SINCE WE WANT TO REFERENCE THIS DATA FOR EVERY SAVE FILE, THIS SCRIPT IS NOT MONOBEHAVIOUR AND IS INSTEAD SERIALIZABLE
    public class CharacterSaveData
    {
        [Header("Character Name")]
        public string characterName = "Character";

        [Header("Time Played")]
        public float secondsPlayed;

        // CAN'T USE VECTOR3 BECAUSE WE CAN ONLY SAVE DATA FROM BASIC OR BUILT IN VARIABLE TYPES
        [Header("World Coordinates")]
        public float xPosition;
        public float yPosition;
        public float zPosition;

        [Header("Resources")]
        public float currentHealth;
        public float currentStamina;

        [Header("Stats")]
        public int vitality;
        public int endurance;

        [Header("Progression")]
        public int playerLevel = 1;
        public int playerExp = 0;
        public int expRequiredForNextLevel = 50;

        [Header("Equipped Gear")]
        // -1 means "nothing equipped in this slot", matching PlayerNetworkManager's netWeaponID/netArmorID
        public int equippedWeaponID = -1;
        public int equippedArmorID = -1;

        [Header("Inventory")]
        public List<InventoryItemSaveData> inventoryItems = new List<InventoryItemSaveData>();
    }
}

