using UnityEngine;
using Unity.Netcode;

namespace NC
{
    public class CharacterManager : NetworkBehaviour
    {
        [Header("Status")]
        public NetworkVariable<bool> isDead = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        [HideInInspector] public CharacterController characterController;
        [HideInInspector] public Animator animator;
        [HideInInspector] public CharacterNetworkManager characterNetworkManager;
        [HideInInspector] public CharacterEffectsManager characterEffectsManager;

        [Header("Flags")]
        public bool isPerformingAction = false;
        // public bool applyRootMotion = false;
        public bool canRotate = true;
        public bool canMove = true;
        public bool isSprinting = false;
        public bool isRolling = false;

        protected virtual void Awake()
        {
            DontDestroyOnLoad(this);
            characterController = GetComponent<CharacterController>();
            animator = GetComponent<Animator>();
            characterNetworkManager = GetComponent<CharacterNetworkManager>();
            characterEffectsManager = GetComponent<CharacterEffectsManager>();
        }

        protected virtual void Update()
        {
            
        }

        protected virtual void LateUpdate() {
            
        }
    
    }
}