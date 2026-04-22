using UnityEngine;

namespace NC
{
    public class TitleScreenLoadMenuInputManager : MonoBehaviour
    {
        PlayerControls playerControls;
        [Header("Title Screen Inputs")]
        [SerializeField] bool deleteCharacterSlot = false;

        void Update()
        {
            if (deleteCharacterSlot)
            {
                deleteCharacterSlot = false;
                TitleScreenManager.Instance.AttemptToDeleteCharacterSlot();
            }
        }

        void OnEnable()
        {
            if (playerControls == null)
            {
                playerControls = new PlayerControls();
                playerControls.UI.uiBtn.performed += i => deleteCharacterSlot = true;
            }
            playerControls.Enable();
        }

        void OnDisable()
        {
            playerControls.Disable();
        }
    }   
}
