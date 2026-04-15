using Unity.Services.Lobbies.Models;
using UnityEngine;

namespace NC
{
    public class PlayerLocomotionManager : CharacterLocomotionManager
    {
        // THIS VALUES ARE TAKEN FROM THE INPUT MANAGER
        PlayerManager playerManager;
        public float verticalMovement;
        public float horizontalMovement;
        public float moveAmount;

        public CharacterNetworkManager characterNetworkManager;

        private Vector3 moveDirection;
        private Vector3 targetRotationDirection;
        [SerializeField] float walkingSpeed = 2f;
        [SerializeField] float runningSpeed = 5f;
        [SerializeField] float rotationSpeed = 15f;

        protected override void Awake()
        {
            base.Awake();

            playerManager = GetComponent<PlayerManager>();
            characterNetworkManager = GetComponent<CharacterNetworkManager>();
        }

        // protected override void Update()
        // {
        //     base.Update();

        //     if (playerManager.IsOwner)
        //     {
        //         // playerManager.characterNetworkManager.verticalMovement.Value = verticalMovement;
        //         // playerManager.characterNetworkManager.horizontalMovement.Value = horizontalMovement;
        //         // playerManager.characterNetworkManager.moveAmount.Value = moveAmount;

        //         characterNetworkManager.verticalMovement.Value = verticalMovement;
        //         characterNetworkManager.horizontalMovement.Value = horizontalMovement;
        //         characterNetworkManager.moveAmount.Value = moveAmount;
        //     }
        //     else
        //     {
        //         // verticalMovement = playerManager.characterNetworkManager.verticalMovement.Value;
        //         // horizontalMovement = playerManager.characterNetworkManager.moveAmount.Value;
        //         // moveAmount = playerManager.characterNetworkManager.moveAmount.Value;

        //         verticalMovement = characterNetworkManager.verticalMovement.Value;
        //         horizontalMovement = characterNetworkManager.moveAmount.Value;
        //         moveAmount = characterNetworkManager.moveAmount.Value;

        //         // IF NOT LOCKED ON, PASS MOVE AMOUNT
        //         playerManager.playerAnimatorManager.UpdateAnimatorMovementParameters(0, moveAmount);

        //         // IF LOCKED ON, PASS HORIZONTAL AND VERTICAL VALUES
        //     }
        // }
        public void HandleAllMovement()
        {
            // GROUNDED MOVEMENT
            HandleGroundedMovement();
            HandleRotation();
            // AERIAL MOVEMENT
        }

        private void GetMovementValues()
        {
            verticalMovement = PlayerInputManager.instance.verticalInput;
            horizontalMovement = PlayerInputManager.instance.horizontalInput;
            moveAmount = PlayerInputManager.instance.moveAmount;

            // CLAMP THE MOVEMENTS
        }

        private void HandleGroundedMovement()
        {
            GetMovementValues();

            // OUR MOVEMENT DIRECTION IS BASED ON OUR CAMERA PERSPECTIVE AND OUR MOVEMENT INPUTS
            moveDirection = PlayerCamera.instance.transform.forward * verticalMovement;
            moveDirection = moveDirection + PlayerCamera.instance.transform.right * horizontalMovement;
            moveDirection.Normalize();
            moveDirection.y = 0;

            if (PlayerInputManager.instance.moveAmount > 0.5f)
            {
                // MOVE AT RUNNING SPEED
                playerManager.characterController.Move(moveDirection * runningSpeed * Time.deltaTime);
            }
            else if(PlayerInputManager.instance.moveAmount <= 0.5f)
            {
                // MOVE AT WALKING SPEED
                playerManager.characterController.Move(moveDirection * walkingSpeed * Time.deltaTime);
            }
        }
        private void HandleRotation()
        {
            targetRotationDirection = Vector3.zero;
            targetRotationDirection = PlayerCamera.instance.cameraObject.transform.forward * verticalMovement;
            targetRotationDirection = targetRotationDirection + PlayerCamera.instance.cameraObject.transform.right * horizontalMovement;
            targetRotationDirection.Normalize();
            targetRotationDirection.y = 0;

            if (targetRotationDirection == Vector3.zero)
            {
                targetRotationDirection = transform.forward;
            }

            Quaternion newRotation = Quaternion.LookRotation(targetRotationDirection);
            Quaternion targetRotation = Quaternion.Slerp(transform.rotation, newRotation, rotationSpeed * Time.deltaTime);
            transform.rotation = targetRotation;
        }
    }
}
