using System.Globalization;
using UnityEngine;

namespace NC
{
    [CreateAssetMenu(menuName = "Character Effects/Instant Effects/Take Damage")]

    public class TakeDamageEffect : InstantCharacterEffect
    {
        [Header("Character Causing Damage")]
        public CharacterManager characterCausingDamage; // If the damage is caused by another character attack, it will be stored here

        [Header("Damage")]
        public float physicalDamage = 0; // Later we can split it into subtypes like, "standard", "slash", "blunt", "pierce", "ranged"
        public float magicDamage = 0;
        public float fireDamage = 0;
        public float iceDamage = 0;
        public float lightMagicDamage = 0;
        public float darkMagicDamage = 0;

        [Header("Final Damage")]
        private int finalDamageDealt = 0; // The damage the character takes after all other damages have been calculated

        [Header("Poise")]
        public float poiseDamage = 0;
        public bool poiseIsBroken = false; // If a characer's poise is broken then it will be stunned or broken and play some animation

        [Header("Sound FX")]
        public bool willPlayDamageSFX = true;
        public AudioClip elementalDamageSoundFX; // Used on top of regular sfx

        [Header("Direction Damage Taken From")]
        public float angleHitFrom; // Used to determine which animation play, i.e , move back, left, right, etc
        public Vector3 contactPoint; // Used to determine where to instantiate the blood or particle hit FX

        // BUILD UPS, later apart from doing damage an attack can ad negative status effect for damaging like poisoning or bleeding
        
        [Header("Animation")]
        public bool playDamageAnimation = true;
        public bool manuallySelectDamageAnimation = false;
        public string damageAnimation; // like a func where set manually to true and replace animation string


        public override void ProcessEffect(CharacterManager character)
        {
            base.ProcessEffect(character);

            // If the char is dead, no additional dmg effects should be processed
            if (character.isDead.Value)
                return;
            
            // Check for char invulnerability, could take various forms

            // Calculate Damage
            CalculateDamage(character);
            // Check which direction the damage came from
            // Play a damage animation
            // Check for build ups
            // Play some damage SFX
            // Play some damage VFX (blood or elemental particles maybe)

            // Is char is AI, Check for new target if character causing damage is present
        }

        private void CalculateDamage(CharacterManager characterManager)
        {
            if (!characterManager.IsOwner)
                return;

            if (characterCausingDamage != null)
            {
                // Check for dmg modifiers and modify base damage (physical, elemental dmg buff)
            }

            // Check character for flat dmg reduction, check flat defenses and substract them from the damage

            // Check char for dmg absorption and substract the percentage from the damage

            //Add all dmg types together, and apply the final damage

            finalDamageDealt = Mathf.RoundToInt(physicalDamage + magicDamage + lightMagicDamage + darkMagicDamage); // this is example, later add all

            if (finalDamageDealt <= 0)
            {
                finalDamageDealt = 1;
            }

            Debug.Log("FINAL DMG GIVEN: " + finalDamageDealt);
            characterManager.characterNetworkManager.currentHealth.Value -= finalDamageDealt;

            // Calculate poise damage to determine if the player will be stunned
        }
    }
}

