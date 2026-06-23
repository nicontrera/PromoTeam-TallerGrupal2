using UnityEngine;

[System.Serializable] // Makes it visible in the Unity Inspector
public class ItemStack
{
    public ItemData item;
    public int quantity;

    // A clean constructor to easily create new stacks
    public ItemStack(ItemData item, int quantity)
    {
        this.item = item;
        this.quantity = quantity;
    }
}