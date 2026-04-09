using Unity.Netcode;
using UnityEngine;

namespace NC
{
    public class PlayerUIManager : MonoBehaviour
    {
        public static PlayerUIManager instance;

        void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }
        private void Start()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}
