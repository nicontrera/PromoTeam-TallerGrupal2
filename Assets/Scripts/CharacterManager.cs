using Unity.VisualScripting;
using UnityEngine;

namespace NC
{
    public class CharacterManager : MonoBehaviour
    {
        protected virtual void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        protected virtual void Update()
        {
            
        }
    }
}