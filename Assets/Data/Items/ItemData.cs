using UnityEngine;

public enum ItemType { Consumable, Weapon, Armor }

// [CreateAssetMenu(fileName = "New Item", menuName = "Inventory/ItemData")]
[CreateAssetMenu(fileName = "ItemData", menuName = "Inventory/ItemData")]

public class ItemData : ScriptableObject
{
    [Header("UI")]
    public int itemID; // CRITICAL for multiplayer: Use this to sync over the network
    public string itemName;
    public ItemType type;
    public Sprite icon;

    [Header("Stacking Properties")]
    public bool isStackable;
    public int maxStackSize = 99; // Default max stack
    
    [Header("Equipment Stats (Weapons/Armor)")]
    public int attackBonus;
    public int defenseBonus;
    public int maxHpBonus;
    public int maxMpBonus;

    [Header("Consumable Stats (Potions)")]
    public int restoreHpAmount;
    public int restoreMpAmount;

    [Header("Equipped Visual")]
    [Tooltip("Model instantiated on the player when this item is equipped - weapon in hand, or armor piece. Leave empty if this item has no visual (e.g. a potion).")]
    public GameObject equipModelPrefab;
}