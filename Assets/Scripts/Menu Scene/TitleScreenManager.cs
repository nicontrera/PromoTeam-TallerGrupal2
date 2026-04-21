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

        [Header("Pop Ups")]
        [SerializeField] GameObject noCharacterSlotsPopUp;
        [SerializeField] Button noCharacterSlotsOkayButton;

        [SerializeField] private float timeoutDuration = 3.0f;

        [Header("Character Slots")]
        public CharacterSlot currentSelectedSlot = CharacterSlot.NO_SLOT;
        

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

        public void StartNetworkAsHost()
        {
            NetworkManager.Singleton.StartHost();
        }
        public void StartNewGameAsClient()
        {
            NetworkManager.Singleton.StartClient();
        }

        public void StartNetworkAsHostOrClient()
        {
            StartCoroutine(TryJoinThenHost());
        }
        public void StartNewGame()
        {
            // StartCoroutine(WorldSaveGameManager.instance.LoadNewGame());
            WorldSaveGameManager.instance.AttemptToCreateNewGame();
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
            WorldSaveGameManager.instance.AttemptToCreateNewGame();
        }
        else
        {
            WorldSaveGameManager.instance.AttemptToCreateNewGame();
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

        public void SelectCharacterSlot(CharacterSlot characterSlot)
        {
            currentSelectedSlot = characterSlot;
        }

        public void SelectNoSlot()
        {
            currentSelectedSlot = CharacterSlot.NO_SLOT;
        }
    }
}
