using UnityEngine;

namespace NC
{
    public class CharacterAnimatorManager : MonoBehaviour
    {
        CharacterManager character;

        public float vertical;
        public float horizontal;

        void Awake()
        {
            character = GetComponent<CharacterManager>();
        }
        public void UpdateAnimatorMovementParameters(float horizontalValue, float verticalValue)
        {
            character.animator.SetFloat("Horizontal", horizontalValue, 0.1f, Time.deltaTime);
            character.animator.SetFloat("Vertical", verticalValue, 0.1f, Time.deltaTime);
        }
    }
}
