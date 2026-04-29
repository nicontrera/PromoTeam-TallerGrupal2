using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using System.Collections;

namespace NC
{
    public class TitleScreenManager : MonoBehaviour
    {
        public static TitleScreenManager Instance;
        [SerializeField] GameObject titleScreenMainMenu;
        [SerializeField] GameObject titleScreenLoadMenu;

        [Header("Buttons")]
        [SerializeField] Button loadMenuReturnButton;
        [SerializeField] Button mainMenuLoadGameButton;
        [SerializeField] Button mainMenuNewGameButton;
        [SerializeField] Button deleteCharacterSlotPopUpConfirmButton;

        [Header("Pop Ups")]
        [SerializeField] GameObject noCharacterSlotsPopUp;
        [SerializeField] Button noCharacterSlotsOkayButton;

        [SerializeField] private float timeoutDuration = 3.0f;
        [SerializeField] public GameObject deleteCharacterSlotPopUp;

        [Header("Character Slots")]
        public CharacterSlot currentSelectedSlot = CharacterSlot.NO_SLOT;
        
        // [Header("Title Screen Inputs")]
        // [SerializeField] bool deleteCharacterSlot = false;

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
        }

        public void StartAsHost()
        {
            Debug.Log("STARTING AS HOST");
            NetworkManager.Singleton.StartHost();
        }
        public void StartAsClient()
        {
            Debug.Log("STARTING AS CLIENT");
            NetworkManager.Singleton.StartClient();
            WorldSaveGameManager.instance.AttemptToCreateNewGame(false);
        }

        public void StartNetworkAsHostOrClient()
        {
            StartCoroutine(TryJoinThenHost());
        }
        public void StartNewGame()
        {
            Debug.Log("STARTING NEW GAME");
            WorldSaveGameManager.instance.AttemptToCreateNewGame(true);
        }

        private IEnumerator TryJoinThenHost()
    {
        Debug.Log("Searching for host...");
        NetworkManager.Singleton.StartClient();

        float timer = 0;
        bool connected = false;

        // Callback to set connected to true
        NetworkManager.Singleton.OnClientConnectedCallback += (id) => {
            if (id == NetworkManager.Singleton.LocalClientId) connected = true;
        };

        // Wait for connection or timeout
        while (timer < timeoutDuration && !connected)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        if (!connected)
        {
            Debug.Log("No host found. Starting as Host instead...");
            NetworkManager.Singleton.Shutdown();
            
            // Wait a frame for shutdown to process
            yield return new WaitUntil(() => !NetworkManager.Singleton.ShutdownInProgress);
            
            NetworkManager.Singleton.StartHost();
            WorldSaveGameManager.instance.AttemptToCreateNewGame(true);
        }
        else
        {
            WorldSaveGameManager.instance.AttemptToCreateNewGame(false);
            Debug.Log("Joined existing host!");
        }
    }


        public void OpenLoadGameMenu()
        {
            titleScreenMainMenu.SetActive(false);
            titleScreenLoadMenu.SetActive(true);

            // FIND THE FIRST LOAD SLOT AND AUTO SELECT IT

            // SELECT THE RETURN BUTTON FIRST
            loadMenuReturnButton.Select();
        }

        public void CloseLoadGameMenu()
        {
            titleScreenLoadMenu.SetActive(false);
            titleScreenMainMenu.SetActive(true);

            // FIND THE FIRST LOAD SLOT AND AUTO SELECT IT

            // SELECT THE LOAD MENU BUTTON FIRST
            mainMenuLoadGameButton.Select();
        }

        public void DisplayNoFreeCharacterSlotsPopUp()
        {
            noCharacterSlotsPopUp.SetActive(true);
            noCharacterSlotsOkayButton.Select();
        }

        public void CloseNoFreeCharacterSlotsPopUp()
        {
            noCharacterSlotsPopUp.SetActive(false);
            mainMenuNewGameButton.Select();
        }

        // CHARACTER SLOTS

        public void SelectCharacterSlot(CharacterSlot characterSlot)
        {
            currentSelectedSlot = characterSlot;
        }

        public void SelectNoSlot()
        {
            currentSelectedSlot = CharacterSlot.NO_SLOT;
        }

        public void AttemptToDeleteCharacterSlot()
        {
            if (currentSelectedSlot != CharacterSlot.NO_SLOT)
            {
                deleteCharacterSlotPopUp.SetActive(true);
                deleteCharacterSlotPopUpConfirmButton.Select();
            }
        }

        public void DeleteCharacterSlot()
        {
            deleteCharacterSlotPopUp.SetActive(false);
            WorldSaveGameManager.instance.DeleteGame(currentSelectedSlot);

            // DISABLE AND ENABLE ACTS AS A REFRESH FOR LOAD MENU, AFTER WE DELETED SOME CHARACTER SLOT
            titleScreenLoadMenu.SetActive(false);
            titleScreenLoadMenu.SetActive(true);

            loadMenuReturnButton.Select();
        }

        public void CloseDeleteCharacterPopUp()
        {
            deleteCharacterSlotPopUp.SetActive(false);
            loadMenuReturnButton.Select();
        }
    }
}
