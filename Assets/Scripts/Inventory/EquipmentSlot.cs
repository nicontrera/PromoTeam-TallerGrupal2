using UnityEngine;
using UnityEngine.UI;

public class EquipmentSlot : MonoBehaviour
{
    public ItemType designatedType; // Set to 'Weapon' on one slot, 'Armor' on the other
    public Image slotIcon;
    public Sprite emptySilhouette;  // The faint gray outline of a sword/shield when empty

    public ItemStack currentStack; 
    private PlayerInventory playerInv;

    public void ConnectInventory(PlayerInventory inv)
    {
        playerInv = inv;
        RefreshSlot();
    }

    public void PutItemInSlot(ItemStack stack)
    {
        currentStack = stack;
        RefreshSlot();
    }

    // Hook this to the UI Button's "On Click()" in the Inspector!
    public void OnSlotClicked()
    {
        // if (currentStack != null && playerInv != null)
        if (currentStack?.item != null && playerInv != null)
        {
            // Send it back to the bag
            playerInv.UnequipGear(designatedType, currentStack);
            currentStack = null;
            RefreshSlot();
        }
    }

    private void RefreshSlot()
    {
        if (currentStack != null && currentStack.item != null)
        {
            slotIcon.sprite = currentStack.item.icon;
            slotIcon.color = Color.white;
        }
        else
        {
            slotIcon.sprite = emptySilhouette;
            slotIcon.color = new Color(1f, 1f, 1f, 0.25f); // 25% transparent gray
        }
    }
}