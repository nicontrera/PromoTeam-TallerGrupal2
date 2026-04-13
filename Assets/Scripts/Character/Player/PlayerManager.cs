using UnityEngine;

namespace NC
{
    public class PlayerManager : CharacterManager
    {
        [HideInInspector] public PlayerAnimatorManager playerAnimatorManager;
        [HideInInspector] public PlayerLocomotionManager playerLocomotionManager;
        protected override void Awake()
        {
            base.Awake();
            // DO MORE STUFF, ONLY FOR THE PLAYER
            playerLocomotionManager = GetComponent<PlayerLocomotionManager>();
            playerAnimatorManager = GetComponent<PlayerAnimatorManager>();
        }

        protected override void Update()
        {
            base.Update();

            // IF WE DO NOT OWN THIS GAMEOBJECT, WE DO NOT CONTROL OR EDIT IT
            if(!IsOwner)
            {
                return;
            }
            // HANDLE ALL OUR CHARACTER MOVEMENT
            playerLocomotionManager.HandleAllMovement();
        }
        protected override void LateUpdate()
        {
            if (!IsOwner)
                return;
            
            base.LateUpdate();

            PlayerCamera.instance.HandleAllCameraActions();
        }
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            //  IF THIS IS THE PLAYER OBJECT OWNED BY THIS CLIENT
            if (IsOwner)
            {
                PlayerInputManager.instance.player = this;
                PlayerCamera.instance.player = this;
            }
        }
    }
}
