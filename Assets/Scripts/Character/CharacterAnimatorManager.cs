using UnityEngine;

namespace NC
{
    public class CharacterAnimatorManager : MonoBehaviour
    {
        CharacterManager character;

        public float vertical;
        public float horizontal;

        protected virtual void Awake()
        {
            character = GetComponent<CharacterManager>();
        }
        public void UpdateAnimatorMovementParameters(float horizontalValue, float verticalValue)
        {
            character.animator.SetFloat("Horizontal", horizontalValue, 0.1f, Time.deltaTime);
            character.animator.SetFloat("Vertical", verticalValue, 0.1f, Time.deltaTime);
        }

        public virtual void PlayTargetActionAnimation(string targetAnimation, bool isPerformingAction, bool applyRootMotion = true, bool canRotate = false, bool canMove = true)
        {
            // character.animator.applyRootMotion = applyRootMotion;
            character.applyRootMotion = applyRootMotion;
            character.animator.CrossFade(targetAnimation, 0.2f);

            // CAN BE USED TO STOP CHARACTER FROM ATTEMPTING NEW ACTIONS
            // FOR EXAMPLE, IF YOU GET DAMAGED, AND BEGIN PERFORMING A TAKEN DAMAGE ANIMATION,
            // THEN THIS FLAG WILL TURN TRUE IF YOU ARE IN STUNNED STATUS
            // WE CAN THEN CHECK FOR THIS BEFORE ATTEMPTING NEW ACTIONS, LIKE A GATEKEEPER
            character.isPerformingAction = isPerformingAction;

            character.canRotate = canRotate;
            // BUG INTERESANTE A CORREGIR, SI EL FLAG ES FALSE SE HACE LA ANIMACION DE ROLL PERO NO SE DESPLAZA NADA EL PLAYER, SI ES TRUE SE PUEDE DESPLAZAR A CUALQUIER LADO COMO SI ESTUVIESE CORRIENDO, QUIERO QUE SE DESPLAZE HACIA ADELANTE UNA CANTIDAD QUE YO ELIJA
            character.canMove = canMove;
        }
    }
}
