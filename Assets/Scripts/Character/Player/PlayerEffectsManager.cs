using Unity.Services.Matchmaker.Models;
using UnityEngine;

namespace NC
{
    public class PlayerEffectsManager : CharacterEffectsManager
    {
        [Header("Debug Delete Later")]
        [SerializeField] InstantCharacterEffect effectToTest;
        [SerializeField] bool processEffect = false;

        void Update()
        {
            if (processEffect)
            {
                processEffect = false;
                // WE INSTANTIATE A COPY OF ORIGINAL SCRIPTABLE OBJECT TO USE THIS ONE AND NOT HAVE TO MODIFY AGAIN THE ORIGINAL NEXT TIME WE USE IT
                InstantCharacterEffect effect = Instantiate(effectToTest);

                // TakeStaminaDamageEffect effect = Instantiate(effectToTest) as TakeStaminaDamageEffect;
                // WHEN WE DONT INSTANTIATE IT, THE ORIGINAL IS CHANGED (DONT WANT THIS IN MOST CASES)
                // effectToTest.staminaDamage = 55;

                ProcessInstantEffect(effect);
            }
        }
    }   
}
