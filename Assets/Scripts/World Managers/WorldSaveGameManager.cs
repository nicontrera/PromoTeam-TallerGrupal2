using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NC
{
    public class WorldSaveGameManager : MonoBehaviour
    {
        public static WorldSaveGameManager instance;

        [SerializeField] private PlayerManager player;

        [Header("SAVE/LOAD")]
        [SerializeField] bool saveGame;
        [SerializeField] bool loadGame;

        [Header("World Scene Index")]
        [SerializeField] int worldSceneIndex = 1;

        [Header("Save Data Writer")]
        private SaveFileDataWriter saveFileDataWriter;

        [Header("Current Character Data")]
        public characterSlot currentCharacterSlotBeingUsed;
        public CharacterSaveData currentCharacterData;
        private string saveFileName;

        [Header("Character Slots")]
        public CharacterSaveData characterSlot01;
        // public CharacterSaveData characterSlot02;
        // public CharacterSaveData characterSlot03;
        // public CharacterSaveData characterSlot04;
        // public CharacterSaveData characterSlot05;
        // public CharacterSaveData characterSlot06;
        // public CharacterSaveData characterSlot07;
        // public CharacterSaveData characterSlot08;
        // public CharacterSaveData characterSlot09;
        // public CharacterSaveData characterSlot10;

        void Awake()
        {
            // THERE CAN ONLY BE ONE INSTANCE OF THIS SCRIPT AT ONE TIME, IF ANOTHER EXISTS, DESTROY IT
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }
        void Start()
        {
            DontDestroyOnLoad(gameObject);
        }

        void Update()
        {
            if (saveGame)
            {
                saveGame = false;
                SaveGame();
            }
            if (loadGame)
            {
                loadGame = false;
                LoadGame();
            }
        }

        public void DecideCharacterFileNameBasedOnSlotBeingUsed()
        {
            switch (currentCharacterSlotBeingUsed)
            {
                case characterSlot.CharacterSlot_01:
                saveFileName = "CharacterSlot_01";
                    break;
                case characterSlot.CharacterSlot_02:
                saveFileName = "CharacterSlot_02";
                    break;
                case characterSlot.CharacterSlot_03:
                saveFileName = "CharacterSlot_03";
                    break;
                case characterSlot.CharacterSlot_04:
                saveFileName = "CharacterSlot_04";
                    break;
                case characterSlot.CharacterSlot_05:
                saveFileName = "CharacterSlot_05";
                    break;
                case characterSlot.CharacterSlot_06:
                saveFileName = "CharacterSlot_06";
                    break;
                case characterSlot.CharacterSlot_07:
                saveFileName = "CharacterSlot_07";
                    break;
                case characterSlot.CharacterSlot_08:
                saveFileName = "CharacterSlot_08";
                    break;
                case characterSlot.CharacterSlot_09:
                saveFileName = "CharacterSlot_09";
                    break;
                case characterSlot.CharacterSlot_10:
                saveFileName = "CharacterSlot_10";
                    break;
                default:
                    break;
            }
        }

        public void CreateNewGame()
        {
            // CREATE A NEW FILE, WITH A FILE NAME DEPENDING ON WHICH SLOT WE ARE USING
            DecideCharacterFileNameBasedOnSlotBeingUsed();
            currentCharacterData = new CharacterSaveData();
        }

        public void LoadGame()
        {
            // LOADING A PREVIOUS FILE, WITH A FILE NAME DEPENDING ON WHICH SLOT WE ARE USING
            DecideCharacterFileNameBasedOnSlotBeingUsed();
            saveFileDataWriter = new SaveFileDataWriter();
            // GENERALLY WORKS ON MULTIPLE MACHINE TYPES
            saveFileDataWriter.saveDataDirectoryPath = Application.persistentDataPath;
            saveFileDataWriter.saveFileName = saveFileName;
            currentCharacterData = saveFileDataWriter.LoadSaveFile();

            StartCoroutine(LoadWorldScene());
        }

        public void SaveGame()
        {
            // SAVE THE CURRENT FILE UNDER A FILE NAME DEPENDING ON WHICH SLOT WE ARE USING
            DecideCharacterFileNameBasedOnSlotBeingUsed();

            saveFileDataWriter = new SaveFileDataWriter();
            // GENERALLY WORKS ON MULTIPLE MACHINE TYPES
            saveFileDataWriter.saveDataDirectoryPath = Application.persistentDataPath;
            saveFileDataWriter.saveFileName = saveFileName;

            // PASS THE PLAYERS INFO, FROM GAME, TO THEIR SAVE FILE
            player.SaveGameDataToCurrentCharacterData(ref currentCharacterData);

            // WRITE THAT INFO ONTO A JSON FILE, SAVED TO THIS MACHINE
            saveFileDataWriter.CreateNewCharacterSaveFile(currentCharacterData);

        }
        public IEnumerator LoadWorldScene()
        {
            AsyncOperation loadOperation = SceneManager.LoadSceneAsync(worldSceneIndex);

            yield return null;
        }
        public int GetWorldSceneIndex()
        {
            return worldSceneIndex;
        }
    }
}