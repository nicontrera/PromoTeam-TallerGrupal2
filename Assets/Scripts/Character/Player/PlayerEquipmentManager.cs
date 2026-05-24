using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace NC
{
    public class PlayerEquipmentManager : CharacterEquipmentManager
    {
        PlayerManager player;

        public WeaponModelInstantiationSlot rightHandSlot;
        public WeaponModelInstantiationSlot leftHandSlot;

        public GameObject rightHandWeaponModel;
        public GameObject leftHandWeaponModel;

        public bool unequipSword = false;

        [Header("Debug delete later")]
        [SerializeField] bool equipNewItems;
        public WeaponItem armaNula;

        [Header("Male Equipment Models")]
        public GameObject maleFullHelmetObject;
        public GameObject[] maleHeadFullHelmets;


        protected override void Awake()
        {
            base.Awake();
            player = GetComponent<PlayerManager>();

            InitializeWeaponSlots();

            List<GameObject> maleFullHelmetsList = new List<GameObject>();

            foreach (Transform child in maleFullHelmetObject.transform)
            {
                maleFullHelmetsList.Add(child.gameObject);
            }

            maleHeadFullHelmets = maleFullHelmetsList.ToArray();
        }

        protected override void Start()
        {
            base.Start();
            
            LoadWeaponsOnBothHands();
        }

        void Update()
        {
            if (Keyboard.current[Key.Digit1].wasPressedThisFrame)
            {
                Debug.Log("key 1 to unnequip");
                rightHandSlot.UnloadWeapon();
            }
            if (Keyboard.current[Key.Digit2].wasPressedThisFrame)
            {
                Debug.Log("key 2 to equip");
                LoadRightWeapon();
            }

            if(equipNewItems)
            {
                equipNewItems = false;
                DebugEquipNewItems();
            }
        }

        private void DebugEquipNewItems()
        {
            Debug.Log("EQUIPPING NEW ITEMS");
            // if(player.playerInventoryManager.headEquipment != null)
            // {
            //     LoadHeadEquipment(player.playerInventoryManager.headEquipment);
            // }
            LoadHeadEquipment(player.playerInventoryManager.headEquipment);
        }

        private void LoadHeadEquipment(HeadEquipmentItem equipment)
        {
            UnloadHeadEquipmentModels();

            if (equipment == null)
            {
                player.playerInventoryManager.headEquipment = null;
                return;
            }

            player.playerInventoryManager.headEquipment = equipment;

            foreach (var model in equipment.equipmentModels)
            {
                model.LoadModel(player, true);
            }
        }

        private void UnloadHeadEquipmentModels()
        {
            foreach (var model in maleHeadFullHelmets)
            {
                model.SetActive(false);
            }


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

        public void LoadWeaponsOnBothHands()
        {
            LoadRightWeapon();
            LoadLeftWeapon();
        }

        public void LoadRightWeapon()
        {
            if (player.playerInventoryManager.currentRightHandWeapon != null)
            {
                rightHandWeaponModel = Instantiate(player.playerInventoryManager.currentRightHandWeapon.weaponModel);
                rightHandSlot.LoadWeapon(rightHandWeaponModel);
            }
        }

        public void LoadLeftWeapon()
        {
            if (player.playerInventoryManager.currentLeftHandWeapon != null)
            {
                leftHandWeaponModel = Instantiate(player.playerInventoryManager.currentLeftHandWeapon.weaponModel);
                leftHandSlot.LoadWeapon(leftHandWeaponModel);
            }
        }
    }
}
