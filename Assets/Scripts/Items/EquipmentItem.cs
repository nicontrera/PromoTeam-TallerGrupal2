using UnityEngine;

namespace NC
{
    public class EquipmentItem : Item
    {
        [Header("Item Weight")]
        public float itemWeight;

        // List of effects that special or normal items can have

        // public void OnItemEquipped(PlayerManager player) - Add all the effects from the list above on the player's effect manager, call this function when this item equips
        // publick void OnItemUnequipped(PlayerManager plauer) - Remove all the effects that were addded, call when this item unequips
    }
}
