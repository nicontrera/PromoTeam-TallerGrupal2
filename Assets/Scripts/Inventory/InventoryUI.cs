using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI Instance; // Singleton for easy access by the local player

    [Header("UI References")]
    public GameObject inventoryPanel; // The visual panel to toggle on/off
    public Transform slotsParent;     // The container that holds the slots (Grid Layout)
    public GameObject slotPrefab;     // The UI prefab for an individual item

    private PlayerInventory localPlayerInventory;
    private List<GameObject> spawnedSlots = new List<GameObject>();


    [Header("Character Sheet Slots")]
    public EquipmentSlot weaponSlotUI;
    public EquipmentSlot armorSlotUI;


    private void Awake()
    {
        // Simple singleton setup
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        inventoryPanel.SetActive(false); // Hide on start
    }

    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        // Toggle inventory with the 'I' key
        // if (Input.GetKeyDown(KeyCode.I))
        if (Keyboard.current != null && Keyboard.current.iKey.wasPressedThisFrame)
        {
            ToggleInventory();
        }
    }

    // Called by the local player when they spawn into the network
    public void ConnectPlayer(PlayerInventory playerInv)
    {
        localPlayerInventory = playerInv;
        localPlayerInventory.OnInventoryChanged += RefreshUI; // Listen for changes

        // NEW: Hook up the equipment boxes to the player
        weaponSlotUI.ConnectInventory(playerInv);
        armorSlotUI.ConnectInventory(playerInv);
    }

    private void ToggleInventory()
    {
        inventoryPanel.SetActive(!inventoryPanel.activeSelf);
        if (inventoryPanel.activeSelf)
        {
            RefreshUI();
        }
    }

    public void RefreshUI()
    {
        if (localPlayerInventory == null) return;

        // 1. Destroy old slots to prevent duplicates
        foreach (GameObject slot in spawnedSlots)
        {
            Destroy(slot);
        }
        spawnedSlots.Clear();

        // 2. Instantiate new slots for every item currently in the list
        foreach (ItemStack stack in localPlayerInventory.inventoryStacks)
        {
            GameObject newSlot = Instantiate(slotPrefab, slotsParent);
            InventorySlot slotScript = newSlot.GetComponent<InventorySlot>();
            
            slotScript.Setup(stack, localPlayerInventory);
            spawnedSlots.Add(newSlot);
        }
    }

    
}