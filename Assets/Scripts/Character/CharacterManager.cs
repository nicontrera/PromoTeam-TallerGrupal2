using UnityEngine;
using Unity.Netcode;

namespace NC
{
    public class CharacterManager : NetworkBehaviour
    {
        [HideInInspector] public CharacterController characterController;
        [HideInInspector] public Animator animator;
        // [HideInInspector] public CharacterNetworkManager characterNetworkManager;

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
            // characterNetworkManager = GetComponent<CharacterNetworkManager>();
        }

        protected virtual void Update()
        {
            
        }

        protected virtual void LateUpdate() {
            
        }
    }
}