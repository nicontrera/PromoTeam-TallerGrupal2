using UnityEngine;

namespace NC
{
    [CreateAssetMenu(menuName = "Character Effects/Instant Effects/Take Stamina Damage")]
    public class TakeStaminaDamageEffect : InstantCharacterEffect
    {
        public float staminaDamage;

        public override void ProcessEffect(CharacterManager character)
        {
            base.ProcessEffect(character);
            CalculateStaminaDamage(character);
        }

        private void CalculateStaminaDamage(CharacterManager character)
        {
            // COMPARE THE BASE STAMINA DAMAGE AGAINST OTHER PLAYER EFFECTS/MODIFIERS
            // CHANGE THE VALUE BEFORE SUBSTRACTING/ADDING IT
            // PLAY SOUND FX OR VFX DURING EFFECT
            if (character.IsOwner)
            {
                Debug.Log("CHAR IS TAKING: " + staminaDamage + "stamina dmg");
                character.characterNetworkManager.currentStamina.Value -= staminaDamage;
            }
        }
    }
}
