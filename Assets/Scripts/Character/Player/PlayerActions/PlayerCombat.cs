using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using NC;

public class PlayerCombat : NetworkBehaviour
{
    [Header("Combat Parameters")]
    public float attackRange = 3f;
    public float attackCooldown = 0.8f; 
    public Transform cameraTransform;   

    private PlayerNetworkManager stats;
    private float nextAttackTime = 0f;

    private void Awake()
    {
        stats = GetComponent<PlayerNetworkManager>();
    }

    private void Update()
    {
        // Only the local player can trigger their own sword swings
        if (!IsOwner) return;

        if (Time.time >= nextAttackTime && Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            // Have we equipped a weapon? (Can't punch a goblin to death just yet)
            if (stats.equippedWeapon == null)
            {
                Debug.LogWarning("Cannot attack: No weapon equipped in your hand!");
                return;
            }

            nextAttackTime = Time.time + attackCooldown;
            SwingSwordLocal();
        }
    }

    private void SwingSwordLocal()
    {
        // 1. Play your local sword-swing animation and WHOOSH audio here later!
        Debug.Log("<color=cyan>[CLIENT]</color> Swung sword!");

        // 2. Calculate the exact origin and trajectory of your camera's center pixel
        Vector3 rayOrigin = cameraTransform ? cameraTransform.position : transform.position + (Vector3.up * 1.5f);
        Vector3 rayDirection = cameraTransform ? cameraTransform.forward : transform.forward;

        // 3. Radio the server to fire the authoritative physics laser
        PerformRaycastAttackServerRpc(rayOrigin, rayDirection);
    }

    [ServerRpc]
    private void PerformRaycastAttackServerRpc(Vector3 origin, Vector3 direction)
    {
        // SERVER AUTHORITY: The server draws the 3-meter line in its own trusted physics universe
        if (Physics.Raycast(origin, direction, out RaycastHit hit, attackRange))
        {
            // Did the laser hit flesh?
            if (hit.collider.TryGetComponent(out EnemyAI goblin))
            {
                int damageToDeal = stats.GetTotalAttack(); 
                
                Debug.Log($"<color=orange>[SERVER]</color> Hit registered! Dealing {damageToDeal} damage to Goblin.");
                
                goblin.TakeDamageServerRpc(damageToDeal);
            }
        }
    }
}