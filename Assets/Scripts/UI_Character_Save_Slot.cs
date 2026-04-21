using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace NC
{
    public class UI_Character_Save_Slot : MonoBehaviour
    {
        SaveFileDataWriter saveFileWriter;

        [Header("Game Slot")]
        public CharacterSlot characterSlot;

        [Header("Character Info")]
        public TextMeshProUGUI characterName;
        public TextMeshProUGUI timePlayed;

        [SerializeField] private float timeoutDuration = 3.0f;


        void OnEnable()
        {
            LoadSaveSlots();
        }

        private void LoadSaveSlots()
        {
            saveFileWriter = new SaveFileDataWriter();
            saveFileWriter.saveDataDirectoryPath = Application.persistentDataPath;

            // switch (characterSlot)
            // {
            //     case CharacterSlot.CharacterSlot_01:
            //     saveFileWriter.saveFileName = WorldSaveGameManager.instance.DecideCharacterFileNameBasedOnSlotBeingUsed(characterSlot);

            //     if (saveFileWriter.CheckToSeeIfFileExists())
            //     {
            //         characterName.text = WorldSaveGameManager.instance.characterSlot01.characterName;
            //     }
            //     else
            //     {
            //         gameObject.SetActive(false);
            //     }
            //         break;
            //     case CharacterSlot.CharacterSlot_02:
            //         break;
            //     case CharacterSlot.CharacterSlot_03:
            //         break;
            //     case CharacterSlot.CharacterSlot_04:
            //         break;
            //     case CharacterSlot.CharacterSlot_05:
            //         break;
            //     case CharacterSlot.CharacterSlot_06:
            //         break;
            //     case CharacterSlot.CharacterSlot_07:
            //         break;
            //     case CharacterSlot.CharacterSlot_08:
            //         break;
            //     case CharacterSlot.CharacterSlot_09:
            //         break;
            //     case CharacterSlot.CharacterSlot_10:
            //         break;
            //     default:
            //         break;
            // }

            // SAVE SLOT 01
            if (characterSlot == CharacterSlot.CharacterSlot_01)
            {
                saveFileWriter.saveFileName = WorldSaveGameManager.instance.DecideCharacterFileNameBasedOnSlotBeingUsed(characterSlot);

                if (saveFileWriter.CheckToSeeIfFileExists())
                {
                    characterName.text = WorldSaveGameManager.instance.characterSlot01.characterName;
                }
                else
                {
                    gameObject.SetActive(false);
                }
            }
            else if (characterSlot == CharacterSlot.CharacterSlot_02)
            {
                saveFileWriter.saveFileName = WorldSaveGameManager.instance.DecideCharacterFileNameBasedOnSlotBeingUsed(characterSlot);

                if (saveFileWriter.CheckToSeeIfFileExists())
                {
                    characterName.text = WorldSaveGameManager.instance.characterSlot02.characterName;
                }
                else
                {
                    gameObject.SetActive(false);
                }
            }
            else if (characterSlot == CharacterSlot.CharacterSlot_03)
            {
                saveFileWriter.saveFileName = WorldSaveGameManager.instance.DecideCharacterFileNameBasedOnSlotBeingUsed(characterSlot);

                if (saveFileWriter.CheckToSeeIfFileExists())
                {
                    characterName.text = WorldSaveGameManager.instance.characterSlot03.characterName;
                }
                else
                {
                    gameObject.SetActive(false);
                }
            }
            else if (characterSlot == CharacterSlot.CharacterSlot_04)
            {
                saveFileWriter.saveFileName = WorldSaveGameManager.instance.DecideCharacterFileNameBasedOnSlotBeingUsed(characterSlot);

                if (saveFileWriter.CheckToSeeIfFileExists())
                {
                    characterName.text = WorldSaveGameManager.instance.characterSlot04.characterName;
                }
                else
                {
                    gameObject.SetActive(false);
                }
            }
            else if (characterSlot == CharacterSlot.CharacterSlot_05)
            {
                saveFileWriter.saveFileName = WorldSaveGameManager.instance.DecideCharacterFileNameBasedOnSlotBeingUsed(characterSlot);

                if (saveFileWriter.CheckToSeeIfFileExists())
                {
                    characterName.text = WorldSaveGameManager.instance.characterSlot05.characterName;
                }
                else
                {
                    gameObject.SetActive(false);
                }
            }
            else if (characterSlot == CharacterSlot.CharacterSlot_06)
            {
                saveFileWriter.saveFileName = WorldSaveGameManager.instance.DecideCharacterFileNameBasedOnSlotBeingUsed(characterSlot);

                if (saveFileWriter.CheckToSeeIfFileExists())
                {
                    characterName.text = WorldSaveGameManager.instance.characterSlot06.characterName;
                }
                else
                {
                    gameObject.SetActive(false);
                }
            }
            else if (characterSlot == CharacterSlot.CharacterSlot_07)
            {
                saveFileWriter.saveFileName = WorldSaveGameManager.instance.DecideCharacterFileNameBasedOnSlotBeingUsed(characterSlot);

                if (saveFileWriter.CheckToSeeIfFileExists())
                {
                    characterName.text = WorldSaveGameManager.instance.characterSlot07.characterName;
                }
                else
                {
                    gameObject.SetActive(false);
                }
            }
            else if (characterSlot == CharacterSlot.CharacterSlot_08)
            {
                saveFileWriter.saveFileName = WorldSaveGameManager.instance.DecideCharacterFileNameBasedOnSlotBeingUsed(characterSlot);

                if (saveFileWriter.CheckToSeeIfFileExists())
                {
                    characterName.text = WorldSaveGameManager.instance.characterSlot08.characterName;
                }
                else
                {
                    gameObject.SetActive(false);
                }
            }
            else if (characterSlot == CharacterSlot.CharacterSlot_09)
            {
                saveFileWriter.saveFileName = WorldSaveGameManager.instance.DecideCharacterFileNameBasedOnSlotBeingUsed(characterSlot);

                if (saveFileWriter.CheckToSeeIfFileExists())
                {
                    characterName.text = WorldSaveGameManager.instance.characterSlot09.characterName;
                }
                else
                {
                    gameObject.SetActive(false);
                }
            }
            else if (characterSlot == CharacterSlot.CharacterSlot_10)
            {
                saveFileWriter.saveFileName = WorldSaveGameManager.instance.DecideCharacterFileNameBasedOnSlotBeingUsed(characterSlot);

                if (saveFileWriter.CheckToSeeIfFileExists())
                {
                    characterName.text = WorldSaveGameManager.instance.characterSlot10.characterName;
                }
                else
                {
                    gameObject.SetActive(false);
                }
            }
        }
    
        public void LoadGameFromCharacterSlot()
        {
            Debug.Log("calling LoadGameFromCharacterSlot");
            WorldSaveGameManager.instance.currentCharacterSlotBeingUsed = characterSlot;
            // WorldSaveGameManager.instance.LoadGame();
            StartCoroutine(TryJoinAsClientOrThenHost());
        }

        public void SelectCurrentSlot()
        {
            TitleScreenManager.Instance.SelectCharacterSlot(characterSlot);
        }

        public void SelectOnHover()
        {
            
        }

        private IEnumerator TryJoinAsClientOrThenHost()
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
            // WorldSaveGameManager.instance.AttemptToCreateNewGame();
            WorldSaveGameManager.instance.LoadGame();
        }
        else
        {
            // WorldSaveGameManager.instance.AttemptToCreateNewGame();
            WorldSaveGameManager.instance.LoadGame();
            Debug.Log("Joined existing host!");
        }
    }
    }
}
