using UnityEngine;

namespace NC
{
    public class ArmorItem : EquipmentItem
    {
        [Header("equipment Absorption Bonus")]
        public float physicalDamageAbsorption;
        public float magicalDamageAbsorption;
        public float fireDamageAbsorption;

        [Header("Equipment Resistance Bonus")]
        public float immunity; // Affects the rot and poison resistance
        public float robustness; // also two
        public float focus;

        [Header("Poise Resistance")]
        public float poiseResistance;

        public EquipmentModel[] equipmentModels;
    }
}
