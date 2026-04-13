using UnityEngine;
using UnityEngine.SceneManagement;

namespace NC
{
    public class PlayerInputManager : MonoBehaviour
    {
        public static PlayerInputManager instance;
        // THINK ABOUT GOALS IN STEPS
        PlayerControls playerControls;

        [Header("PLAYER MOVEMENT INPUT")]
        [SerializeField] Vector2 movementInput;
        public float horizontalInput;
        public float verticalInput;
        public float moveAmount;

        [Header("CAMERA MOVEMENT INPUT")]
        [SerializeField] Vector2 cameraInput;
        public float cameraHorizontalInput;
        public float cameraVerticalInput;

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
        }

        void Start()
        {
            DontDestroyOnLoad(gameObject);

            // WHEN SCENE CHANGES, RUN THIS LOGIC
            SceneManager.activeSceneChanged += OnSceneChange;

            instance.enabled = false;
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
            HandlePlayerInputMovement();
            HandleCameraMovementInput();
        }
        private void HandlePlayerInputMovement()
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
        }

        private void HandleCameraMovementInput()
        {
            cameraVerticalInput = cameraInput.y;
            cameraHorizontalInput = cameraInput.x;

        }
    }
}
