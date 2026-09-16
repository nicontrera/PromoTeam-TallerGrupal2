using UnityEngine;

namespace NC
{
    public class PlayerTargetingManager : MonoBehaviour
    {
        PlayerManager player;

        [Header("Current Lock")]
        public CharacterManager currentTarget;

        [Header("Lock-On Visual Indicator")]
        [Tooltip("Drag a simple prefab here - e.g. a GameObject with a SpriteRenderer using LockOnReticle.png.")]
        [SerializeField] GameObject lockOnIndicatorPrefab;
        [SerializeField] Vector3 indicatorOffset = new Vector3(0f, 2f, 0f); // Height above the target's pivot
        private GameObject currentIndicatorInstance;

        [Header("Lock-On Settings")]
        [SerializeField] float lockOnRange = 15f;
        [SerializeField] float lockOnConeAngle = 45f; // How far off-crosshair a target can be and still get picked
        [SerializeField] float maxLockDistance = 20f; // Auto-unlock if the target ends up further than this
        [Tooltip("Layers a lock-on search is allowed to pick as a target - e.g. your Enemy layer.")]
        [SerializeField] LayerMask targetableLayers;
        [Tooltip("Layers that block line-of-sight to a potential target - e.g. Environment/Default.")]
        [SerializeField] LayerMask lineOfSightBlockingLayers;

        protected virtual void Awake()
        {
            player = GetComponent<PlayerManager>();
        }

        void Update()
        {
            if (currentTarget == null)
                return;

            if (!IsTargetStillValid(currentTarget))
            {
                ClearTarget();
                return;
            }

            UpdateIndicator();
        }

        // CALLED BY PlayerInputManager WHEN THE LOCK-ON INPUT IS PRESSED
        public void ToggleLockOn()
        {
            // ALREADY LOCKED - PRESSING AGAIN CLEARS IT
            if (currentTarget != null)
            {
                ClearTarget();
                return;
            }

            CharacterManager bestTarget = FindBestTarget();

            if (bestTarget != null)
            {
                currentTarget = bestTarget;
                SpawnIndicator();
            }
        }

        public void ClearTarget()
        {
            currentTarget = null;
            DestroyIndicator();
        }

        private void SpawnIndicator()
        {
            if (lockOnIndicatorPrefab == null || currentTarget == null)
                return;

            currentIndicatorInstance = Instantiate(lockOnIndicatorPrefab, GetIndicatorPosition(), Quaternion.identity);
        }

        private void DestroyIndicator()
        {
            if (currentIndicatorInstance != null)
            {
                Destroy(currentIndicatorInstance);
                currentIndicatorInstance = null;
            }
        }

        private Vector3 GetIndicatorPosition()
        {
            return currentTarget.transform.position + indicatorOffset;
        }

        private void UpdateIndicator()
        {
            if (currentIndicatorInstance == null)
                return;

            currentIndicatorInstance.transform.position = GetIndicatorPosition();

            // BILLBOARD: ALWAYS FACE THE CAMERA SO A FLAT SPRITE READS CORRECTLY FROM ANY ANGLE
            Transform camTransform = PlayerCamera.instance.cameraObject.transform;
            currentIndicatorInstance.transform.forward = camTransform.forward;
        }

        private void OnDisable()
        {
            DestroyIndicator();
        }

        private CharacterManager FindBestTarget()
        {
            Transform cameraTransform = PlayerCamera.instance.cameraObject.transform;

            Collider[] candidates = Physics.OverlapSphere(transform.position, lockOnRange, targetableLayers);

            CharacterManager bestTarget = null;
            float bestAngle = lockOnConeAngle; // Only accept candidates inside this cone from the crosshair

            foreach (Collider candidate in candidates)
            {
                CharacterManager character = candidate.GetComponentInParent<CharacterManager>();

                if (character == null || character == player)
                    continue;

                if (!IsTargetStillValid(character))
                    continue;

                Vector3 directionToTarget = (character.transform.position - cameraTransform.position).normalized;
                float angle = Vector3.Angle(cameraTransform.forward, directionToTarget);

                if (angle < bestAngle && HasLineOfSightTo(character))
                {
                    bestAngle = angle;
                    bestTarget = character;
                }
            }

            return bestTarget;
        }

        private bool HasLineOfSightTo(CharacterManager target)
        {
            Vector3 origin = transform.position + Vector3.up * 1.5f;
            Vector3 targetPosition = target.transform.position + Vector3.up * 1f;
            Vector3 direction = targetPosition - origin;

            // If something solid sits between us and the target, we can't lock onto it
            if (Physics.Raycast(origin, direction.normalized, out RaycastHit hit, direction.magnitude, lineOfSightBlockingLayers))
                return false;

            return true;
        }

        private bool IsTargetStillValid(CharacterManager target)
        {
            if (target == null)
                return false;

            // Enemies expose their own death state through EnemyAI - drop the lock once they're dead
            if (target.TryGetComponent(out EnemyAI enemyAI) && enemyAI.state.Value == EnemyAI.AIState.Dead)
                return false;

            if (Vector3.Distance(transform.position, target.transform.position) > maxLockDistance)
                return false;

            return true;
        }
    }
}
