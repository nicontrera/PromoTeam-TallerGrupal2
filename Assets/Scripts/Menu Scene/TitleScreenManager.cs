using UnityEngine;
using Unity.Netcode;

namespace NC
{
    public class TitleScreenManager : MonoBehaviour
    {
        public void StartNetworkAsHost()
        {
            NetworkManager.Singleton.StartHost();
        }

        public void StartNewGame()
        {
            // StartCoroutine(WorldSaveGameManager.instance.LoadNewGame());
            WorldSaveGameManager.instance.CreateNewGame();
            StartCoroutine(WorldSaveGameManager.instance.LoadWorldScene());
        }

        public void StartNewGameAsClient()
        {
            NetworkManager.Singleton.StartClient();
        }
    }
}
