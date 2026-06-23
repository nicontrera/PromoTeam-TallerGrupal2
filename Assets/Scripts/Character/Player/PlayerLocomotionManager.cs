using System.Collections;
using Unity.Netcode;
using UnityEngine;

namespace NC
{
    public class PlayerLocomotionManager : CharacterLocomotionManager
    {
        // THIS VALUES ARE TAKEN FROM THE INPUT MANAGER
        PlayerManager playerManager;

        [Header("Attack Settings")]
        [SerializeField] float attackStaminaCost = 15f;
        public float raycastAttackRange = 3.5f; // Range for the camera laser

        [HideInInspector] public float verticalMovement;
        [HideInInspector] public float horizontalMovement;
        [HideInInspector] public float moveAmount;

        // public CharacterNetworkManager characterNetworkManager;

        [Header("Movement Settings")]
        private Vector3 moveDirection;
        private Vector3 targetRotationDirection;
        [SerializeField] float walkingSpeed = 2f;
        [SerializeField] float runningSpeed = 4f;
        [SerializeField] float sprintingSpeed = 40f;
        [SerializeField] float rotationSpeed = 15f;
        [SerializeField] int sprintingStaminaCost = 2;
        

        [Header("Dodge")]
        private Vector3 rollDirection;
        [SerializeField] float dodgeStaminaCost = 20;

        public Transform attackPoint; // Un objeto vacío frente al jugador
        public float attackRange = 0.5f; // Radio del golpe
        public LayerMask enemyLayers; // Para no pegarle al suelo o a ti mismo
        public int attackDamage = 20;

        protected override void Awake()
        {
            base.Awake();

            playerManager = GetComponent<PlayerManager>();
            // characterNetworkManager = GetComponent<CharacterNetworkManager>();
        }

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

            if (playerManager.isSprinting)
            {
                playerManager.characterController.Move(moveDirection * sprintingSpeed * Time.deltaTime);
            }
            else
            {
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

            if (playerManager.playerNetworkManager.currentStamina.Value <= 0)
                return;
            // GetMovementValues();

            // IF WE ARE MOVING WHEN WE ATTEMP TO PERFORM A DODFE WE ROLL
            if (moveAmount > 0) // INSTEAD OF MOVEAMOUNT CAN USE  PlayerInputManager.instance.moveAmount
            {
                // rollDirection = PlayerCamera.instance.cameraObject.transform.forward * verticalMovement;
                rollDirection = PlayerCamera.instance.cameraObject.transform.forward * PlayerInputManager.instance.verticalInput;
                rollDirection += PlayerCamera.instance.cameraObject.transform.right * PlayerInputManager.instance.horizontalInput;
                rollDirection.y = 0;
                rollDirection.Normalize();

                Quaternion playerRollRotation = Quaternion.LookRotation(rollDirection);
                playerManager.transform.rotation = playerRollRotation;

                // StartCoroutine(Dash());

                // PERFORM A ROLL ANIMATION
                playerManager.playerAnimatorManager.PlayTargetActionAnimation("Roll_Forward_04", true, true); // THIRD PARAMETER HAS A DEFAULT TRUE VALUE, 02?
            }
            // IF WE ARE STATIONARY, WE PERFORM A BACKSTEP
            else
            {
                // StartCoroutine(BackStep());

                // PERFORM A BACKSTEP ANIMATION
                playerManager.playerAnimatorManager.PlayTargetActionAnimation("Back_Step_02", true, true); // 01?
            }
            playerManager.playerNetworkManager.currentStamina.Value -= dodgeStaminaCost;

        }

        IEnumerator Dash()
        {
            float startTime = Time.time;

            Vector3 rollDirection;

            rollDirection = PlayerCamera.instance.cameraObject.transform.forward * 1;
            rollDirection += PlayerCamera.instance.cameraObject.transform.right * 0;
            rollDirection.y = 0;
            rollDirection.Normalize();

            while(Time.time < startTime + 0.6f)
            {
                playerManager.characterController.Move(rollDirection * 18f * Time.deltaTime);
                yield return null;
            }
        }

        IEnumerator BackStep()
        {
            float startTime = Time.time;

            Vector3 rollDirection;

            rollDirection = PlayerCamera.instance.cameraObject.transform.forward * -1;
            rollDirection += PlayerCamera.instance.cameraObject.transform.right * 0;
            rollDirection.y = 0;
            rollDirection.Normalize();

            while(Time.time < startTime + 0.4f)
            {
                playerManager.characterController.Move(rollDirection * 20f * Time.deltaTime);
                yield return null;
            }
        }

        public void HandleSprinting()
        {
            if (playerManager.isPerformingAction)
            {
                // SET SPRINTING TO FALSE
                playerManager.isSprinting = false;
            }

            // IF WE ARE OUT OF STAMINA, SET SPRINTING TO FALSE
            if (playerManager.playerNetworkManager.currentStamina.Value <= 0)
            {
                playerManager.isSprinting = false;
                return;
            }

            // IF WE ARE MOVING SPRINTING IS TRUE
            if (moveAmount >= 0.5 && !playerManager.isPerformingAction)
            {
                playerManager.isSprinting = true;    
                // Debug.Log("setting isSprinting as true, is ok?");      
            }
            // IF WE ARE STATIONARY OR MOVING SLOWLY, SPRINTING IS FALSE
            else
            {
                playerManager.isSprinting = false;           
            }

            if (playerManager.isSprinting)
            {
                playerManager.playerNetworkManager.currentStamina.Value -= sprintingStaminaCost * Time.deltaTime;
            }

        }


        // public void Handle1HBasicAttack()
        // {
        //     ulong thisPlayerId = NetworkManager.Singleton.LocalClientId;

        //     if (playerManager.isPerformingAction)
        //         return;

        //     if (playerManager.playerNetworkManager.currentStamina.Value <= 0)
        //         return;

        //     playerManager.playerAnimatorManager.PlayTargetActionAnimationTrigger("Attack", true, true);

        //     // 2. Detectar enemigos en el rango
        //     // Crea una esfera invisible y guarda lo que toque en un array
        //     Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayers);

        //     // 3. Aplicar daño a cada enemigo detectado
        //     foreach (Collider enemy in hitEnemies)
        //     {
        //         Debug.Log("Golpeaste a: " + enemy.name);
                
        //         // Aquí llamamos a una función en el script del enemigo
        //         if (enemy.GetComponent<EnemyManager>() != null) {
        //             enemy.GetComponent<EnemyManager>().TakeDamage(attackDamage, thisPlayerId);
        //             // Debug.Log("The player id is: " + thisPlayerId);

        //             if (enemy.GetComponent<EnemyManager>().enemyNetworkManager.currentEnemyHealth.Value <= 0)
        //             {
        //                 playerManager.CheckForLevelUpOk(30, thisPlayerId);
        //             }
        //         }
        //     }
        // }


        public void Handle1HBasicAttack()
        {
            ulong thisPlayerId = NetworkManager.Singleton.LocalClientId;

            if (playerManager.isPerformingAction)
                return;

            // Verify we have enough stamina to swing
            if (playerManager.playerNetworkManager.currentStamina.Value < attackStaminaCost)
                return;

            // 1. Deduct Stamina instantly on the client for a responsive UI
            playerManager.playerNetworkManager.currentStamina.Value -= attackStaminaCost;

            // 2. Play attack animation gatekeeper
            playerManager.playerAnimatorManager.PlayTargetActionAnimationTrigger("Attack", true, true);

            // 3. Grab the live inventory damage (Base Attack + Weapon Bonus)
            int finalAttackDamage = playerManager.playerNetworkManager.GetTotalAttack();

            // 4. Perform the Screen-Center Raycast from the Camera perspective
            Transform camTransform = PlayerCamera.instance.transform;
            if (Physics.Raycast(camTransform.position, camTransform.forward, out RaycastHit hit, raycastAttackRange, enemyLayers))
            {
                Debug.Log($"Laser aimed successfully! Hit collider: {hit.collider.name}");

                if (hit.collider.TryGetComponent(out NetworkObject enemyNetObj))
                {
                    // Radio the Server to confirm the hit and apply damage globally!
                    Debug.Log($"TESTING 1ST IF");

                    playerManager.playerNetworkManager.NotifyAttackHitServerRpc(enemyNetObj.NetworkObjectId, finalAttackDamage, thisPlayerId);
                }
            }
        }

        // Para poder ver el rango de ataque en el editor de Unity
        void OnDrawGizmosSelected()
        {
            if (attackPoint == null) return;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }
    }
}
