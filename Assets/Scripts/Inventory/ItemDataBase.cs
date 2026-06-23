using UnityEngine;
using System.Collections.Generic;

public class ItemDatabase : MonoBehaviour
{
    // Singleton instance for global access without needing FindObjectOfType
    private static ItemDatabase instance;

    [Header("Master Item List")]
    [Tooltip("Drag all your ItemData ScriptableObjects here in the Unity Inspector")]
    public List<ItemData> allItems = new List<ItemData>();

    // Dictionary for ultra-fast server lookups
    private Dictionary<int, ItemData> itemLookup = new Dictionary<int, ItemData>();

    private void Awake()
    {
        // Enforce the Singleton pattern
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        instance = this;
        
        // Optional: Keep this alive across scene loads (useful for open worlds)
        DontDestroyOnLoad(gameObject); 

        InitializeDatabase();
    }

    // void Start()
    // {
    //     DontDestroyOnLoad(gameObject);
    // }

    private void InitializeDatabase()
    {
        itemLookup.Clear();
        
        foreach (ItemData item in allItems)
        {
            if (item == null) continue;

            // Safety check: Prevent accidental duplicate IDs in the inspector
            if (!itemLookup.ContainsKey(item.itemID))
            {
                itemLookup.Add(item.itemID, item);
            }
            else
            {
                Debug.LogError($"ItemDatabase Error: Duplicate ID {item.itemID} found on {item.itemName}! Please assign a unique ID.");
            }
        }
        
        Debug.Log($"ItemDatabase loaded successfully with {itemLookup.Count} items.");
    }

    /// <summary>
    /// Retrieves an ItemData reference from its integer ID. 
    /// Used primarily by the server during RPCs.
    /// </summary>
    public static ItemData GetItemByID(int id)
    {
        // Fast lookup via TryGetValue
        if (instance != null && instance.itemLookup.TryGetValue(id, out ItemData item))
        {
            return item;
        }
        
        Debug.LogWarning($"ItemDatabase: Item with ID {id} does not exist in the database.");
        return null;
    }
}