using System.Collections.Generic;
using UnityEngine;

namespace NC
{
    public class WorldCharacterEffectsManager : MonoBehaviour
    {
        public static WorldCharacterEffectsManager instance;

        [Header("Damage")]
        public TakeDamageEffect takeDamageEffect;
        // public InstantCharacterEffect instantCharacterEffect;
        [SerializeField] List<InstantCharacterEffect> instantCharacterEffects;

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
            GenerateEffectsIDs();
        }

        // void Start()
        // {
        //     DontDestroyOnLoad(gameObject);
        // }

        private void GenerateEffectsIDs()
        {
            for (int i = 0; i < instantCharacterEffects.Count; i++)
            {
                instantCharacterEffects[i].instantEffectID = i;
            }
        }
    }
}

