using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlot : MonoBehaviour
{
    public Image iconImage;
    // public Sprite iconImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI quantityText; // Link this in the inspector!
    
    private ItemStack currentStack;
    private PlayerInventory playerInventory;

    // Updated to accept ItemStack
    public void Setup(ItemStack stack, PlayerInventory inventory)
    {
        currentStack = stack;
        playerInventory = inventory;

        iconImage.sprite = stack.item.icon;
        // iconImage = stack.item.icon;
        nameText.text = stack.item.itemName;

        // Only show numbers if it's stackable AND we have more than 1
        if (stack.item.isStackable && stack.quantity > 1)
        {
            quantityText.text = stack.quantity.ToString();
            quantityText.gameObject.SetActive(true);
        }
        else
        {
            quantityText.gameObject.SetActive(false);
        }
    }

    public void OnSlotClicked()
    {
        if (currentStack != null && playerInventory != null)
        {
            playerInventory.UseItem(currentStack);
        }
    }


//     public void OnSlotClicked()
// {
//     // TRAP 1: Did the physical click even make it to this script?
//     Debug.Log("<color=cyan>[UI TRAP]</color> The slot button was physically clicked!");

//     if (currentStack != null && playerInventory != null)
//     {
//         Debug.Log($"<color=green>[SUCCESS]</color> Passing {currentStack.item.itemName} x{currentStack.quantity} to PlayerInventory...");
//         playerInventory.UseItem(currentStack);
//     }
//     else
//     {
//         // TRAP 2: The click worked, but our data references are missing!
//         Debug.LogWarning($"<color=yellow>[SILENT FAIL]</color> currentStack is null? {currentStack == null} | playerInventory is null? {playerInventory == null}");
//     }
// }
}