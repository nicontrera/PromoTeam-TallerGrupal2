using UnityEngine;

namespace NC
{
    public class PlayerLocomotionManager : CharacterLocomotionManager
    {
        // THIS VALUES ARE TAKEN FROM THE INPUT MANAGER
        PlayerManager playerManager;

        [HideInInspector] public float verticalMovement;
        [HideInInspector] public float horizontalMovement;
        [HideInInspector] public float moveAmount;

        public CharacterNetworkManager characterNetworkManager;

        [Header("Movement Settings")]
        private Vector3 moveDirection;
        private Vector3 targetRotationDirection;
        [SerializeField] float walkingSpeed = 2f;
        [SerializeField] float runningSpeed = 5f;
        [SerializeField] float rotationSpeed = 15f;

        [Header("Dodge")]
        private Vector3 rollDirection;

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
            if (!playerManager.canMove)
                return;

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
            if (!playerManager.canRotate)
                return;
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

        public void AttempToPerformDodge()
        {
            if (playerManager.isPerformingAction)
                return;

            // IF WE ARE MOVING WHEN WE ATTEMP TO PERFORM A DODFE WE ROLL
            if (moveAmount > 0) // INSTEAD OF MOVEAMOUNT CAN USE  PlayerInputManager.instance.moveAmount
            {
                rollDirection = PlayerCamera.instance.cameraObject.transform.forward * verticalMovement;
                rollDirection += PlayerCamera.instance.cameraObject.transform.right * horizontalMovement;
                rollDirection.y = 0;
                rollDirection.Normalize();

                Quaternion playerRollRotation = Quaternion.LookRotation(rollDirection);
                playerManager.transform.rotation = playerRollRotation;

                // PERFORM A ROLL ANIMATION
                playerManager.playerAnimatorManager.PlayTargetActionAnimation("Roll_Forward_01", true, true); // THIRD PARAMETER HAS A DEFAULT TRUE VALUE
            }
            // IF WE ARE STATIONARY, WE PERFORM A BACKSTEP
            else
            {
                // PERFORM A BACKSTEP ANIMATION
                playerManager.playerAnimatorManager.PlayTargetActionAnimation("Back_Step_01", true, true);
            }
        }
    }
}
