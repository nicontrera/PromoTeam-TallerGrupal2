using UnityEngine;

namespace NC
{
    public class CharacterStatsManager : MonoBehaviour
    {
        CharacterManager character;

        [Header("Stamina Regeneration")]
        [SerializeField] float staminaRegenerationAmount = 2f;
        private float staminaRegenerationTimer = 0f;
        // this regeneration can greatly change the speed and pace of the game
        private float staminaTickTimer = 0f;
        [SerializeField] float staminaRegenerationDelay = 2f;

        protected virtual void Awake()
        {
            character = GetComponent<CharacterManager>();
        }

        protected virtual void Start() {
            
        }

        public int CalculateHealthBasedOnVitalityLevel(int vitality)
        {
            float health = 0;

            // CREATE AN EQUATION FOR HOW YOU WANT YOUR STAMINA TO BE CALCULATED

            health = vitality * 10;

            return  Mathf.RoundToInt(health);
        }

        public int CalculateStaminaBasedOnEnduranceLevel(int endurance)
        {
            float stamina = 0;

            // CREATE AN EQUATION FOR HOW YOU WANT YOUR STAMINA TO BE CALCULATED

            stamina = endurance * 10;

            return  Mathf.RoundToInt(stamina);
        }

        public virtual void RegenerateStamina()
        {
            // ONLY OWNERS CAN EDIT THEIR NETWORK VARIABLES
            if (!character.IsOwner)
                return;
            
            // WE DO NOT WANT TO REGENERATE STAMINA IF WE ARE USING IT
            if (character.isSprinting)
                return;

            if (character.isPerformingAction)
                return;

            staminaRegenerationTimer += Time.deltaTime;

            if (staminaRegenerationTimer >= staminaRegenerationDelay)
            {
                if (character.characterNetworkManager.currentStamina.Value < character.characterNetworkManager.maxStamina.Value)
                {
                    staminaTickTimer += Time.deltaTime;

                    if (staminaTickTimer >= 0.1) // REGEN EACH 1/10 OF A SECOND
                    {
                        staminaTickTimer = 0;
                        character.characterNetworkManager.currentStamina.Value += staminaRegenerationAmount; // THIS AMOUNT
                    }
                }
            }
        }

        public virtual void ResetStaminaRegenTimer(float previousStaminaAmount, float currentStaminaAmount)
        {
            // WE ONLY WANT TO RESET REGENERATION IF THE ACTION USED STAMINA
            // WE ONLY WANT TO RESET REGENERATION IF WE ARE ALREADY REGENERATING STAMINA
            if (currentStaminaAmount < previousStaminaAmount)
            {
                staminaRegenerationTimer = 0;
                
            }

        }
    }
}
