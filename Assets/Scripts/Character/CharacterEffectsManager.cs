using UnityEngine;

namespace NC
{
    public class CharacterEffectsManager : MonoBehaviour
    {
        // PROCESS INSTANT EFFECTS (TAKE DMG, HEAL, ETC)

        // PROCESS TIMED EFFECTS (POISON, BUILD UPS)

        // PROCESS STATIC EFFECTS (ADDING/REMOVING BUFFS FROM TALISMANS, ETC)

        CharacterManager character;

        protected virtual void Awake() {
            character = GetComponent<CharacterManager>();
        }

        public virtual void ProcessInstantEffect(InstantCharacterEffect effect)
        {
            // TAKE IN AN EFFECT
            // PROCESS IT
            effect.ProcessEffect(character);
        }
    }
}
