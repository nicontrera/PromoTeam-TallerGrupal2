using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NC
{
    public class WorldSaveGameManager : MonoBehaviour
    {
        public static WorldSaveGameManager instance;

        public PlayerManager player;

        [Header("SAVE/LOAD")]
        [SerializeField] bool saveGame;
        [SerializeField] bool loadGame;
        [SerializeField] bool loadOnlyCharData;

        [Header("World Scene Index")]
        [SerializeField] int worldSceneIndex = 1;

        [Header("Save Data Writer")]
        private SaveFileDataWriter saveFileDataWriter;

        [Header("Current Character Data")]
        public CharacterSlot currentCharacterSlotBeingUsed;
        public CharacterSaveData currentCharacterData;
        private string saveFileName;

        [Header("Character Slots")]
        public CharacterSaveData characterSlot01;
        public CharacterSaveData characterSlot02;
        public CharacterSaveData characterSlot03;
        public CharacterSaveData characterSlot04;
        public CharacterSaveData characterSlot05;
        public CharacterSaveData characterSlot06;
        public CharacterSaveData characterSlot07;
        public CharacterSaveData characterSlot08;
        public CharacterSaveData characterSlot09;
        public CharacterSaveData characterSlot10;

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
            LoadAllCharacterProfiles();
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
            if (loadOnlyCharData)
            {
                loadOnlyCharData = false;
                SubirStats();
            }
        }

        public string DecideCharacterFileNameBasedOnSlotBeingUsed(CharacterSlot characterSlot)
        {
            string fileName = "";
            switch (characterSlot)
            {
                case CharacterSlot.CharacterSlot_01:
                fileName = "CharacterSlot_01";
                    break;
                case CharacterSlot.CharacterSlot_02:
                fileName = "CharacterSlot_02";
                    break;
                case CharacterSlot.CharacterSlot_03:
                fileName = "CharacterSlot_03";
                    break;
                case CharacterSlot.CharacterSlot_04:
                fileName = "CharacterSlot_04";
                    break;
                case CharacterSlot.CharacterSlot_05:
                fileName = "CharacterSlot_05";
                    break;
                case CharacterSlot.CharacterSlot_06:
                fileName = "CharacterSlot_06";
                    break;
                case CharacterSlot.CharacterSlot_07:
                fileName = "CharacterSlot_07";
                    break;
                case CharacterSlot.CharacterSlot_08:
                fileName = "CharacterSlot_08";
                    break;
                case CharacterSlot.CharacterSlot_09:
                fileName = "CharacterSlot_09";
                    break;
                case CharacterSlot.CharacterSlot_10:
                fileName = "CharacterSlot_10";
                    break;
                default:
                    break;
            }
            return fileName;
        }

        public void AttemptToCreateNewGame(bool isHost)
        {
            saveFileDataWriter = new SaveFileDataWriter();
            saveFileDataWriter.saveDataDirectoryPath = Application.persistentDataPath;

            // CHECK TO SEE IF WE CAN CREATE A NEW SAVE FILE (CHECK FOR OTHER EXISTING FILES FIRST)
            saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnSlotBeingUsed(CharacterSlot.CharacterSlot_01);

            if (!saveFileDataWriter.CheckToSeeIfFileExists())
            {
                // IF THIS PROFILE SLOT IS NOT TAKEN, MAKE A NEW ONE USING THIS SLOT
                currentCharacterSlotBeingUsed = CharacterSlot.CharacterSlot_01;
                currentCharacterData = new CharacterSaveData();
                if (isHost)
                {
                    Debug.Log("IS HOST TRUE");
                    NewGame();
                    return;
                }
                // else
                // {
                //     Debug.Log("IS HOST FALSE");
                //     player.playerNetworkManager.vitality.Value = 15;
                //     player.playerNetworkManager.endurance.Value = 10;
                //     SaveGame();
                //     LoadOnlyCharacterData();
                // }
            }
            
            saveFileName = DecideCharacterFileNameBasedOnSlotBeingUsed(CharacterSlot.CharacterSlot_02);

            if (!saveFileDataWriter.CheckToSeeIfFileExists())
            {
                // IF THIS PROFILE SLOT IS NOT TAKEN, MAKE A NEW ONE USING THIS SLOT
                currentCharacterSlotBeingUsed = CharacterSlot.CharacterSlot_02;
                currentCharacterData = new CharacterSaveData();
                if (isHost)
                {
                    Debug.Log("IS HOST TRUE");
                    NewGame();
                    return;
                }
            //     else
            //     {
            //         Debug.Log("IS HOST FALSE");
            //         player.playerNetworkManager.vitality.Value = 15;
            // player.playerNetworkManager.endurance.Value = 10;
            //         SaveGame();
            //         LoadOnlyCharacterData();
            //     }
            }

            // CHECK TO SEE IF WE CAN CREATE A NEW SAVE FILE (CHECK FOR OTHER EXISTING FILES FIRST)
            saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnSlotBeingUsed(CharacterSlot.CharacterSlot_03);

            if (!saveFileDataWriter.CheckToSeeIfFileExists())
            {
                // IF THIS PROFILE SLOT IS NOT TAKEN, MAKE A NEW ONE USING THIS SLOT
                currentCharacterSlotBeingUsed = CharacterSlot.CharacterSlot_03;
                currentCharacterData = new CharacterSaveData();
                if (isHost)
                {
                    Debug.Log("IS HOST TRUE");
                    NewGame();
                    return;
                }
            //     else
            //     {
            //         Debug.Log("IS HOST FALSE");
            //         player.playerNetworkManager.vitality.Value = 15;
            // player.playerNetworkManager.endurance.Value = 10;
            //         SaveGame();
            //         LoadOnlyCharacterData();
            //     }
            }

            // CHECK TO SEE IF WE CAN CREATE A NEW SAVE FILE (CHECK FOR OTHER EXISTING FILES FIRST)
            saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnSlotBeingUsed(CharacterSlot.CharacterSlot_04);

            if (!saveFileDataWriter.CheckToSeeIfFileExists())
            {
                // IF THIS PROFILE SLOT IS NOT TAKEN, MAKE A NEW ONE USING THIS SLOT
                currentCharacterSlotBeingUsed = CharacterSlot.CharacterSlot_04;
                currentCharacterData = new CharacterSaveData();
                if (isHost)
                {
                    Debug.Log("IS HOST TRUE");
                    NewGame();
                    return;
                }
            //     else
            //     {
            //         player.playerNetworkManager.vitality.Value = 15;
            // player.playerNetworkManager.endurance.Value = 10;
            //         Debug.Log("IS HOST FALSE");
            //         SaveGame();
            //         LoadOnlyCharacterData();
            //     }
            }

            // CHECK TO SEE IF WE CAN CREATE A NEW SAVE FILE (CHECK FOR OTHER EXISTING FILES FIRST)
            saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnSlotBeingUsed(CharacterSlot.CharacterSlot_05);

            if (!saveFileDataWriter.CheckToSeeIfFileExists())
            {
                // IF THIS PROFILE SLOT IS NOT TAKEN, MAKE A NEW ONE USING THIS SLOT
                currentCharacterSlotBeingUsed = CharacterSlot.CharacterSlot_05;
                currentCharacterData = new CharacterSaveData();
                if (isHost)
                {
                    Debug.Log("IS HOST TRUE");
                    NewGame();
                    return;
                }
            //     else
            //     {
            //         player.playerNetworkManager.vitality.Value = 15;
            // player.playerNetworkManager.endurance.Value = 10;
            //         Debug.Log("IS HOST FALSE");
            //         SaveGame();
            //         LoadOnlyCharacterData();
            //     }
            }

            // CHECK TO SEE IF WE CAN CREATE A NEW SAVE FILE (CHECK FOR OTHER EXISTING FILES FIRST)
            saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnSlotBeingUsed(CharacterSlot.CharacterSlot_06);

            if (!saveFileDataWriter.CheckToSeeIfFileExists())
            {
                // IF THIS PROFILE SLOT IS NOT TAKEN, MAKE A NEW ONE USING THIS SLOT
                currentCharacterSlotBeingUsed = CharacterSlot.CharacterSlot_06;
                currentCharacterData = new CharacterSaveData();
                if (isHost)
                {
                    Debug.Log("IS HOST TRUE");
                    NewGame();
                    return;
                }
                // else
                // {
                //     player.playerNetworkManager.vitality.Value = 15;
                //     player.playerNetworkManager.endurance.Value = 10;
                //     Debug.Log("IS HOST FALSE");
                //     SaveGame();
                //     LoadOnlyCharacterData();
                // }
            }

            // CHECK TO SEE IF WE CAN CREATE A NEW SAVE FILE (CHECK FOR OTHER EXISTING FILES FIRST)
            saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnSlotBeingUsed(CharacterSlot.CharacterSlot_07);

            if (!saveFileDataWriter.CheckToSeeIfFileExists())
            {
                // IF THIS PROFILE SLOT IS NOT TAKEN, MAKE A NEW ONE USING THIS SLOT
                currentCharacterSlotBeingUsed = CharacterSlot.CharacterSlot_07;
                currentCharacterData = new CharacterSaveData();
                if (isHost)
                {
                    Debug.Log("IS HOST TRUE");
                    NewGame();
                    return;
                }
            //     else
            //     {
            //         player.playerNetworkManager.vitality.Value = 15;
            // player.playerNetworkManager.endurance.Value = 10;
            //         Debug.Log("IS HOST FALSE");
            //         SaveGame();
            //         LoadOnlyCharacterData();
            //     }
            }

            // CHECK TO SEE IF WE CAN CREATE A NEW SAVE FILE (CHECK FOR OTHER EXISTING FILES FIRST)
            saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnSlotBeingUsed(CharacterSlot.CharacterSlot_08);

            if (!saveFileDataWriter.CheckToSeeIfFileExists())
            {
                // IF THIS PROFILE SLOT IS NOT TAKEN, MAKE A NEW ONE USING THIS SLOT
                currentCharacterSlotBeingUsed = CharacterSlot.CharacterSlot_08;
                currentCharacterData = new CharacterSaveData();
                if (isHost)
                {
                    Debug.Log("IS HOST TRUE");
                    NewGame();
                    return;
                }
            //     else
            //     {
            //         player.playerNetworkManager.vitality.Value = 15;
            // player.playerNetworkManager.endurance.Value = 10;
            //         Debug.Log("IS HOST FALSE");
            //         SaveGame();
            //         LoadOnlyCharacterData();
            //     }
            }

            // CHECK TO SEE IF WE CAN CREATE A NEW SAVE FILE (CHECK FOR OTHER EXISTING FILES FIRST)
            saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnSlotBeingUsed(CharacterSlot.CharacterSlot_09);

            if (!saveFileDataWriter.CheckToSeeIfFileExists())
            {
                // IF THIS PROFILE SLOT IS NOT TAKEN, MAKE A NEW ONE USING THIS SLOT
                currentCharacterSlotBeingUsed = CharacterSlot.CharacterSlot_09;
                currentCharacterData = new CharacterSaveData();
                if (isHost)
                {
                    Debug.Log("IS HOST TRUE");
                    NewGame();
                    return;
                }
            //     else
            //     {
            //         player.playerNetworkManager.vitality.Value = 15;
            // player.playerNetworkManager.endurance.Value = 10;
            //         Debug.Log("IS HOST FALSE");
            //         SaveGame();
            //         LoadOnlyCharacterData();
            //     }
            }

            // CHECK TO SEE IF WE CAN CREATE A NEW SAVE FILE (CHECK FOR OTHER EXISTING FILES FIRST)
            saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnSlotBeingUsed(CharacterSlot.CharacterSlot_10);

            if (!saveFileDataWriter.CheckToSeeIfFileExists())
            {
                // IF THIS PROFILE SLOT IS NOT TAKEN, MAKE A NEW ONE USING THIS SLOT
                currentCharacterSlotBeingUsed = CharacterSlot.CharacterSlot_10;
                currentCharacterData = new CharacterSaveData();
                // StartCoroutine(LoadWorldScene());
                if (isHost)
                {
                    Debug.Log("IS HOST TRUE");
                    NewGame();
                    return;
                }
                // else
                // {
                //     player.playerNetworkManager.vitality.Value = 15;
                //     player.playerNetworkManager.endurance.Value = 10;
                //     Debug.Log("IS HOST FALSE");
                //     SaveGame();
                //     LoadOnlyCharacterData();
                // }
            }

            // CREATE A NEW FILE, WITH A FILE NAME DEPENDING ON WHICH SLOT WE ARE USING

            // IF THERE ARE NOT FREE SLOTS, NOTIFY THE PLAYER, CAN'T USE UNTIL DELETES OR OVERWRITES ONE
            TitleScreenManager.Instance.DisplayNoFreeCharacterSlotsPopUp();
        }

        private void NewGame()
        {
            // SAVES THE NEWLY CREATED CHARACTER STATS, AND ITEMS (MAYBE LATER WHEN CREATION MENU SCREEN IS ADDED)

            player.playerNetworkManager.vitality.Value = 15;
            player.playerNetworkManager.endurance.Value = 10;

            SaveGame();
            StartCoroutine(LoadWorldScene());
        }

        public void LoadGame()
        {
            // LOADING A PREVIOUS FILE, WITH A FILE NAME DEPENDING ON WHICH SLOT WE ARE USING
            saveFileName = DecideCharacterFileNameBasedOnSlotBeingUsed(currentCharacterSlotBeingUsed);
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
            saveFileName = DecideCharacterFileNameBasedOnSlotBeingUsed(currentCharacterSlotBeingUsed);

            saveFileDataWriter = new SaveFileDataWriter();
            // GENERALLY WORKS ON MULTIPLE MACHINE TYPES
            saveFileDataWriter.saveDataDirectoryPath = Application.persistentDataPath;
            saveFileDataWriter.saveFileName = saveFileName;

            // PASS THE PLAYERS INFO, FROM GAME, TO THEIR SAVE FILE
            player.SaveGameDataToCurrentCharacterData(ref currentCharacterData);

            // WRITE THAT INFO ONTO A JSON FILE, SAVED TO THIS MACHINE
            saveFileDataWriter.CreateNewCharacterSaveFile(currentCharacterData);

        }

        public void DeleteGame(CharacterSlot characterSlot)
        {
            saveFileDataWriter = new SaveFileDataWriter();
            saveFileDataWriter.saveDataDirectoryPath = Application.persistentDataPath;
            // CHOOSE FILE BASED ON NAME
            saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnSlotBeingUsed(characterSlot);

            
            // saveFileDataWriter.saveFileName = saveFileName;
            saveFileDataWriter.DeleteSaveFile();
        }

        // LOAD ALL CHARACTER PROFILES SAVED ON DEVICE WHEN STARTING GAME
        private void LoadAllCharacterProfiles()
        {
            saveFileDataWriter = new SaveFileDataWriter();
            
            saveFileDataWriter.saveDataDirectoryPath = Application.persistentDataPath;

            saveFileDataWriter.saveFileName =  DecideCharacterFileNameBasedOnSlotBeingUsed(CharacterSlot.CharacterSlot_01);
            characterSlot01 = saveFileDataWriter.LoadSaveFile();

            saveFileDataWriter.saveFileName =  DecideCharacterFileNameBasedOnSlotBeingUsed(CharacterSlot.CharacterSlot_02);
            characterSlot02 = saveFileDataWriter.LoadSaveFile();

            saveFileDataWriter.saveFileName =  DecideCharacterFileNameBasedOnSlotBeingUsed(CharacterSlot.CharacterSlot_03);
            characterSlot03 = saveFileDataWriter.LoadSaveFile();

            saveFileDataWriter.saveFileName =  DecideCharacterFileNameBasedOnSlotBeingUsed(CharacterSlot.CharacterSlot_04);
            characterSlot04 = saveFileDataWriter.LoadSaveFile();

            saveFileDataWriter.saveFileName =  DecideCharacterFileNameBasedOnSlotBeingUsed(CharacterSlot.CharacterSlot_05);
            characterSlot05 = saveFileDataWriter.LoadSaveFile();

            saveFileDataWriter.saveFileName =  DecideCharacterFileNameBasedOnSlotBeingUsed(CharacterSlot.CharacterSlot_06);
            characterSlot06 = saveFileDataWriter.LoadSaveFile();

            saveFileDataWriter.saveFileName =  DecideCharacterFileNameBasedOnSlotBeingUsed(CharacterSlot.CharacterSlot_07);
            characterSlot07 = saveFileDataWriter.LoadSaveFile();

            saveFileDataWriter.saveFileName =  DecideCharacterFileNameBasedOnSlotBeingUsed(CharacterSlot.CharacterSlot_08);
            characterSlot08 = saveFileDataWriter.LoadSaveFile();

            saveFileDataWriter.saveFileName =  DecideCharacterFileNameBasedOnSlotBeingUsed(CharacterSlot.CharacterSlot_09);
            characterSlot09 = saveFileDataWriter.LoadSaveFile();

            saveFileDataWriter.saveFileName =  DecideCharacterFileNameBasedOnSlotBeingUsed(CharacterSlot.CharacterSlot_10);
            characterSlot10 = saveFileDataWriter.LoadSaveFile();
        }
        public IEnumerator LoadWorldScene()
        {
            AsyncOperation loadOperation = SceneManager.LoadSceneAsync(worldSceneIndex);

            player.LoadGameDataFromCurrentCharacterData(ref currentCharacterData);

            yield return null;
        }

        public void LoadOnlyCharacterData()
        {
            player.LoadGameDataFromCurrentCharacterData(ref currentCharacterData);
            
        }

        private void SubirStats()
        {
            player.playerNetworkManager.vitality.Value += 10;
            player.playerNetworkManager.endurance.Value += 10;
        }
        public int GetWorldSceneIndex()
        {
            return worldSceneIndex;
        }
    }
}