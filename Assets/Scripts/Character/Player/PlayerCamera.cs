using UnityEngine;

namespace NC
{
    public class PlayerCamera : MonoBehaviour
    {
        public static PlayerCamera instance;
        public PlayerManager player;
        public Camera cameraObject;
        [SerializeField] Transform cameraPivotTransform;

        // CHANGE THESE TO TWEAK CAMERA PERFORMANCE
        [Header("Camera Settings")]
        private float cameraSmoothSpeed = 1f; // THE BIGGER THIS NUMBER, THE LONGER FOR THE CAMERA TO REACH ITS POSITION
        [SerializeField] float leftAndRightRotationSpeed = 220f;
        [SerializeField] float upAndDownRotationSpeed = 220f;
        [SerializeField] float minimumPivot = -30; // THE LOWEST POINT YOU ARE ABLE TO LOOK DOWN
        [SerializeField] float maximumPivot = 60; // THE HIGHEST POINT YOU ARE ABLE TO LOOK UP
        [SerializeField] float cameraCollisionRadius = 0.2f;
        [SerializeField] LayerMask collideWithLayers;


        [Header("Camera Values")]
        private Vector3 cameraVelocity;
        private Vector3 cameraObjectPosition; // USED FOR CAMERA COLLISIONS (MOVES THE CAMERA OBJECT TO THIS POSITION UPON COLLIDING)
        [SerializeField] float leftAndRightLookAngle;
        [SerializeField] float upAndDownLookAngle;
        private float cameraZPosition; // VALUES USED FOR THE CAMERA COLLISION
        private float targetCameraZPosition; // VALUES USED FOR THE CAMERA COLLISION

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
            cameraZPosition = cameraObject.transform.localPosition.z;
        }

        public void HandleAllCameraActions()
        {
            if (player != null)
            {
                // FOLLOW THE PLAYER (TARGET)
                HandleFollowTarget();
                // ROTATE AROUND THE PLAYER (TARGET)
                HandleRotations();
                // COLLIDE WITH OBJECTS (ENVIROMENT) AND IF NEEDED PASS THROUGH WALLS
                HandleCollisions();
            }
        }

        private void HandleFollowTarget()
        {
            Vector3 targetCameraZPosition = Vector3.SmoothDamp(transform.position, player.transform.position, ref cameraVelocity, cameraSmoothSpeed * Time.deltaTime);
            transform.position = targetCameraZPosition;
        }

        private void HandleRotations()
        {
            // IF LOCKED ON, FORCE ROTATIONS TOWARDS TARGET
            // ELSE ROTATE REGULARLY

            // ROTATE LEFT AND RIGHT BASED ON HORIZONTAL MOVEMENT OF THE MOUSE
            leftAndRightLookAngle += (PlayerInputManager.instance.cameraHorizontalInput * leftAndRightRotationSpeed) * Time.deltaTime;
            // ROTATE UP AND DOWN BASED ON VERTICAL MOVEMENT OF THE MOUSE
            upAndDownLookAngle -= (PlayerInputManager.instance.cameraVerticalInput * upAndDownRotationSpeed) * Time.deltaTime;
            // CLAMP THE UP AND DOWN LOOK ANGLE BETWEEN MIN AND MAX VALUE
            upAndDownLookAngle = Mathf.Clamp(upAndDownLookAngle, minimumPivot, maximumPivot);

            Vector3 cameraRotation;
            Quaternion targetRotation;
            // ROTATE THIS GAMEOBJECT LEFT AND RIGHT
            cameraRotation = Vector3.zero;
            cameraRotation.y = leftAndRightLookAngle;
            targetRotation = Quaternion.Euler(cameraRotation);
            transform.rotation = targetRotation;

            // ROTATE THE PIVOT GAMEOBJECT UP AND DOWN
            cameraRotation = Vector3.zero;
            cameraRotation.x = upAndDownLookAngle;
            targetRotation = Quaternion.Euler(cameraRotation);
            cameraPivotTransform.localRotation = targetRotation;
        }

        private void HandleCollisions()
        {
            targetCameraZPosition = cameraZPosition;
            RaycastHit hit;
            // DESIRED DIRECTION, DIRECTION FOR COLLISION CHECK
            Vector3 direction = cameraObject.transform.position - cameraPivotTransform.position;
            direction.Normalize();

            // WE CHECK IF THERE IS AN OBJECT IN FRONT OF OUR DESIRED DIRECTION (SEE ABOVE)
            if (Physics.SphereCast(cameraPivotTransform.position, cameraCollisionRadius, direction, out hit, Mathf.Abs(targetCameraZPosition), collideWithLayers))
            {
                // IF THERE IS, WE GET OUR DISTANCE FROM IT
                float distanceFromObject = Vector3.Distance(cameraPivotTransform.position, hit.point);
                // WE THEN EQUATE OUR TARGET Z POSITION TO THE FOLLOWING
                targetCameraZPosition = -(distanceFromObject - cameraCollisionRadius);
            }

            // IF OUR TARGET POSITION IS LESS THAN OUR COLLISION RADIUS, WE SUBSTRACT OUR COLLISION (MAKING IT SNAP BACK)
            if (Mathf.Abs(cameraZPosition) < cameraCollisionRadius)
            {
                targetCameraZPosition = -cameraCollisionRadius;
            }

            // WE THEN APPLY OUT FINAL POSITION USING LERP OVER A TIME OF 0.2F
            cameraObjectPosition.z = Mathf.Lerp(cameraObject.transform.localPosition.z, targetCameraZPosition, cameraCollisionRadius);
            cameraObject.transform.localPosition = cameraObjectPosition;
        }
    }
}

