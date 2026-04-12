using UnityEngine;
using UnityEngine.SceneManagement;

namespace NC
{
    public class PlayerInputManager : MonoBehaviour
    {
        public static PlayerInputManager instance;
        // THINK ABOUT GOALS IN STEPS
        PlayerControls playerControls;
        [SerializeField] Vector2 movementInput;
        [SerializeField] float horizontalInput;
        [SerializeField] float verticalInput;
        [SerializeField] float moveAmount;

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
            }

            playerControls.Enable();
        }

        private void OnDestroy()
        {
            // IF WE DESTROY THIS OBJECT, UNSUBSCRIBE FROM THIS EVENT
            SceneManager.activeSceneChanged -= OnSceneChange;
        }
        private void HandleInputMovement()
        {
            verticalInput = movementInput.y;
            horizontalInput = movementInput.x;

            moveAmount = Mathf.Clamp01(Mathf.Abs(verticalInput) + Mathf.Abs(horizontalInput));
        }
    }
    
}
