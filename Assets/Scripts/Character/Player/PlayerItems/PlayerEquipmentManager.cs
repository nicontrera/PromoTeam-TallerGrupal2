using UnityEngine;

namespace NC
{
    public class PlayerEquipmentManager : CharacterEquipmentManager
    {
        PlayerManager player;

        [Header("Weapon Socket")]
        public WeaponModelInstantiationSlot rightHandSlot;
        public WeaponModelInstantiationSlot leftHandSlot; // Not used yet - single weapon slot for now, matches PlayerNetworkManager.equippedWeapon

        private GameObject rightHandWeaponModel;

        [Header("Armor Socket")]
        [Tooltip("Where armor visuals attach - e.g. an empty Transform on the character's head or chest bone.")]
        public Transform armorSocket;

        private GameObject currentArmorModel;

        protected override void Awake()
        {
            base.Awake();
            player = GetComponent<PlayerManager>();

            InitializeWeaponSlots();
        }

        private void InitializeWeaponSlots()
        {
            WeaponModelInstantiationSlot[] weaponSlots = GetComponentsInChildren<WeaponModelInstantiationSlot>();

            foreach (var weaponSlot in weaponSlots)
            {
                if (weaponSlot.weaponSlot == WeaponModelSlot.RightHand)
                {
                    rightHandSlot = weaponSlot;
                }
                else if (weaponSlot.weaponSlot == WeaponModelSlot.LeftHand)
                {
                    leftHandSlot = weaponSlot;
                }
            }
        }

        // CALLED BY PlayerNetworkManager.TranslateIDToGear WHENEVER netWeaponID CHANGES
        // (INCLUDING ON SPAWN, SO LATE JOINERS AND YOU YOURSELF SEE CURRENT GEAR IMMEDIATELY)
        public void LoadWeaponVisual(ItemData weapon)
        {
            UnloadWeaponVisual();

            if (weapon == null || weapon.equipModelPrefab == null || rightHandSlot == null)
                return;

            rightHandWeaponModel = Instantiate(weapon.equipModelPrefab);
            rightHandSlot.LoadWeapon(rightHandWeaponModel);
        }

        public void UnloadWeaponVisual()
        {
            if (rightHandSlot != null)
                rightHandSlot.UnloadWeapon();

            rightHandWeaponModel = null;
        }

        // CALLED BY PlayerNetworkManager.TranslateIDToGear WHENEVER netArmorID CHANGES
        public void LoadArmorVisual(ItemData armor)
        {
            UnloadArmorVisual();

            if (armor == null || armor.equipModelPrefab == null || armorSocket == null)
                return;

            currentArmorModel = Instantiate(armor.equipModelPrefab, armorSocket);
            currentArmorModel.transform.localPosition = Vector3.zero;
            currentArmorModel.transform.localRotation = Quaternion.identity;
            currentArmorModel.transform.localScale = Vector3.one;
        }

        public void UnloadArmorVisual()
        {
            if (currentArmorModel != null)
            {
                Destroy(currentArmorModel);
                currentArmorModel = null;
            }
        }
    }
}
