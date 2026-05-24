using UnityEngine;

namespace NC
{
    [CreateAssetMenu(menuName = "Equipment Model")]
    public class EquipmentModel : ScriptableObject
    {
        public EquipmentModelType equipmentModelType;
        public string maleEquipmentName;
        public string femaleEquipmentName;

        public void LoadModel(PlayerManager player, bool isMale)
        {
            if (isMale)
            {
                LoadMaleModel(player);
            }
            else
            {
                LoadFemaleModel(player);
            }
        }

        public void LoadMaleModel(PlayerManager player)
        {
            switch (equipmentModelType)
            {
                case EquipmentModelType.FullHelmet:
                foreach (var model in player.playerEquipmentManager.maleHeadFullHelmets)
                {
                    if (model.gameObject.name == maleEquipmentName)
                    {
                        model.gameObject.SetActive(true);
                        // if prefered can add some material here (model.gameobject.getcomponent<Renderer>().material = Instantiate(equipmentMaterial))
                    }
                }
                    break;
                case EquipmentModelType.OpenHelmet:
                    break;
                case EquipmentModelType.Hood:
                    break;
                default:
                    break;
            }
        }

        public void LoadFemaleModel(PlayerManager player)
        {
            
        }
    }
}
