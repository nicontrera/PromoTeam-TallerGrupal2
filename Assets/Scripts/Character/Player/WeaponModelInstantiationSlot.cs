using UnityEngine;

namespace NC
{
    public class WeaponModelInstantiationSlot : MonoBehaviour
    {
        // What slot? (left, right, back, hip?)
        public WeaponModelSlot weaponSlot;
        public GameObject currentWeaponModel;

        private void UnloadWeapon()
        {
            if (currentWeaponModel != null)
            {
                Destroy(currentWeaponModel);
            }
        }

        public void LoadWeapon(GameObject weaponModel)
        {
            currentWeaponModel = weaponModel;
            weaponModel.transform.parent = transform;

            weaponModel.transform.localPosition = Vector3.zero;
            weaponModel.transform.localRotation = Quaternion.identity;
            weaponModel.transform.localScale = Vector3.one;
        }
    }
}
