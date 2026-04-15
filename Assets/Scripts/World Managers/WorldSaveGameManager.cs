using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NC
{
    public class WorldSaveGameManager : MonoBehaviour
    {
        public static WorldSaveGameManager instance;
        [SerializeField] int worldSceneIndex = 1;

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
        public IEnumerator LoadNewGame()
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