using UnityEngine;
using UnityEngine.SceneManagement;

namespace NC
{
    public class PlayerInputManager : MonoBehaviour
    {
        public static PlayerInputManager instance;
        public PlayerManager player;
        // THINK ABOUT GOALS IN STEPS
        PlayerControls playerControls;

        [Header("CAMERA MOVEMENT INPUT")]
        [SerializeField] Vector2 cameraInput;
        public float cameraHorizontalInput;
        public float cameraVerticalInput;

        [Header("PLAYER MOVEMENT INPUT")]
        [SerializeField] Vector2 movementInput;
        public float horizontalInput;
        public float verticalInput;
        public float moveAmount;

        [Header("PLAYER ACTIONS INPUT")]
        [SerializeField] bool dodgeInput = false;
        [SerializeField] bool sprintInput;
        [SerializeField] Vector3 animatorInfo;


        void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
            // player = GetComponent<PlayerManager>();
        }

        void Start()
        {
            DontDestroyOnLoad(gameObject);

            // WHEN SCENE CHANGES, RUN THIS LOGIC
            SceneManager.activeSceneChanged += OnSceneChange;

            instance.enabled = false;

            // player = new PlayerManager();
        }

        private void OnSceneChange(Scene oldScene, Scene newScene)
        {
            // IF WE ARE LOADING INTO OUR WORLD SCENE, ENABLE OUR PLAYERS CONTROLS
            if (newScene.buildIndex == WorldSaveGameManager.instance.GetWorldSceneIndex())
            {
                instance.enabled = true;
            }
            // OTHERWISE WE MUST BE AT THE MAIN MENU, DISABLE OUR PLAYER CONTROLS
            // THIS IS SO OUT PLAYER CANT MOVE AROUND IF WE ENTER THINGS LIKE CHARACTER CREATION MENU, ETC
            else
            {
                instance.enabled = false;
            }
        }

        void OnEnable()
        {
            if (playerControls == null)
            {
                playerControls = new PlayerControls();

                playerControls.PlayerMovement.Movement.performed += i => movementInput = i.ReadValue<Vector2>();
                playerControls.PlayerCamera.Movement.performed += i => cameraInput = i.ReadValue<Vector2>();
                playerControls.PlayerActions.Dodge.performed += i => dodgeInput = true;

                // HOLDING THE INPUT SETS THE BOOL TO TRUE
                playerControls.PlayerActions.Sprint.performed += i => sprintInput = true;
                // RELEASING THE INPUT SETS THE BOOL TO FALSE
                playerControls.PlayerActions.Sprint.canceled += i => sprintInput = false;
            }

            playerControls.Enable();
        }

        private void OnDestroy()
        {
            // IF WE DESTROY THIS OBJECT, UNSUBSCRIBE FROM THIS EVENT
            SceneManager.activeSceneChanged -= OnSceneChange;
        }

        void OnApplicationFocus(bool focus)
        {
            if (enabled)
            {
                if (focus)
                {
                    playerControls.Enable();
                }
                else
                {
                    playerControls.Disable();
                }
            }
        }
        void Update()
        {
            HandleAllInputs();
        }

        private void HandleAllInputs()
        {
            HandlePlayerMovementInput();
            HandleCameraMovementInput();
            HandleDodge();
            HandleSprinting();
        }

        // MOVEMENT
        private void HandlePlayerMovementInput()
        {
            verticalInput = movementInput.y;
            horizontalInput = movementInput.x;

            moveAmount = Mathf.Clamp01(Mathf.Abs(verticalInput) + Mathf.Abs(horizontalInput));

            // WE CLAMP THE VALUES, SO THEY  ARE 0.5 OR 1 (OPTIONAL)
            if (moveAmount <= 0.5 && moveAmount > 0)
            {
                moveAmount = 0.5f;
            }
            else if(moveAmount > 0.5 && moveAmount <= 1)
            {
                moveAmount = 1;
            }

            if (player == null)
                return;

            player.playerAnimatorManager.UpdateAnimatorMovementParameters(0, moveAmount, player.isSprinting);
        }

        private void HandleCameraMovementInput()
        {
            cameraVerticalInput = cameraInput.y;
            cameraHorizontalInput = cameraInput.x;

        }

        // ACTIONS
        private void HandleDodge()
        {
            if (dodgeInput)
            {
                dodgeInput = false;
                // NOTE FOR FUTURE: RETURN (DO NOTHING) IF MENU OR INVENTORY WINDOWS IS ACTIVE/OPEN
                // PERFORM A DODGE

                // animatorInfo = player.animator.deltaPosition;
                // Debug.Log(animatorInfo);
                player.playerLocomotionManager.AttempToPerformDodge();

                // StartCoroutine(Dash());
            }
        }

        // IEnumerator Dash()
        // {
        //     float startTime = Time.time;

        //     Vector3 rollDirection;

        //     rollDirection = PlayerCamera.instance.cameraObject.transform.forward * 1;
        //     rollDirection += PlayerCamera.instance.cameraObject.transform.right * 0;
        //     rollDirection.y = 0;
        //     rollDirection.Normalize();

        //     while(Time.time < startTime + 0.4f)
        //     {
        //         player.characterController.Move(rollDirection * 20f * Time.deltaTime);
        //         yield return null;
        //     }
        // }

        private void HandleSprinting()
        {
            if (sprintInput)
            {
                // MAKE PLAYER SPRINTS
                player.isSprinting = true;
                player.playerLocomotionManager.HandleSprinting();
            }
            else
            {
                player.isSprinting = false;
            }
        }
    }
}
