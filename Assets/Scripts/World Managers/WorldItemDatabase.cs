using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR;

namespace NC
{
    public class WorldItemDatabase : MonoBehaviour
    {
        public static WorldItemDatabase Instance;
        public WeaponItem unarmedWeapon;

        [Header("Weapons")]
        [SerializeField] List<WeaponItem> weapons = new List<WeaponItem>();

        [Header("Head Equipment")]
        [SerializeField] List<HeadEquipmentItem> headEquipment = new List<HeadEquipmentItem>();

        [Header("Body Equipment")]
        [SerializeField] List<BodyEquipmentItem> bodyEquipment = new List<BodyEquipmentItem>();

        [Header("Leg Equipment")]
        [SerializeField] List<LegEquipmentItem> legEquipment = new List<LegEquipmentItem>();

        [Header("Hand Equipment")]
        [SerializeField] List<HandEquipmentItem> handEquipment = new List<HandEquipmentItem>();

        // List of every item we have in the game
        [Header("Items")]
        private List<Item> items = new List<Item>();

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            // Add weapons to list of items
            foreach (var weapon in weapons)
            {
                items.Add(weapon);
            }

            foreach (var item in headEquipment)
            {
                items.Add(item);
            }

            foreach (var item in bodyEquipment)
            {
                items.Add(item);
            }

            foreach (var item in legEquipment)
            {
                items.Add(item);
            }

            foreach (var item in handEquipment)
            {
                items.Add(item);
            }

            // Assign a unique item ID to our items
            for (int i = 0; i < items.Count; i++)
            {
                items[i].itemID = i;
            }
        }

        public WeaponItem GetWeaponByID(int ID)
        {
            return weapons.FirstOrDefault(weapon => weapon.itemID == ID);
        }

        public HeadEquipmentItem GetHeadEquipmentByID(int ID)
        {
            return headEquipment.FirstOrDefault(equipment => equipment.itemID == ID);
        }

        public BodyEquipmentItem GetBodyEquipmentByID(int ID)
        {
            return bodyEquipment.FirstOrDefault(equipment => equipment.itemID == ID);
        }

        public LegEquipmentItem GetLegEquipmentByID(int ID)
        {
            return legEquipment.FirstOrDefault(equipment => equipment.itemID == ID);
        }

        public HandEquipmentItem GetHandEquipmentByID(int ID)
        {
            return handEquipment.FirstOrDefault(equipment => equipment.itemID == ID);
        }
    }   
}
