using UnityEngine;

namespace NC
{
    public class WeaponItem : EquipmentItem
    {
        // Animator controller override (Change attack animation based on the weapon is using)

        [Header("Weapon Model")]
        public GameObject weaponModel;

        [Header("Weapon Requirements")]
        public int strReq;
        public int dexReq;
        public int intReq;

        [Header("Weapon Base Damage")]
        public int physicalDamage = 0;
        public int magicDamage = 0;
        public int lightMagicDamage = 0;
        public int darkMagicDamage = 0;

        // Weapong defensive stance absorption (blocking power)

        [Header("Weapon Base Poise Damage")]
        public float poiseDamage = 10;
        // Offensive poise dmg bonus when attacking

        // Weapon modifiers
        // Light attack, Heavy attack, Critical Hit, etc

        [Header("Stamina Cost")]
        public int baseStaminaCost = 10;
        // Running attack stamina cost
        // Light attack stamina cost, Heavy atttack stamina cost, etc

        // Items based actions (R,F,T), blocking sound

    }
}
