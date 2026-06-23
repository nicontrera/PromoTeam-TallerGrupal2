using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;
using NC;

public class EnemyAI : NetworkBehaviour
{
    public enum AIState { Idle, Chasing, Attacking, Dead }

    [Header("Melee Weapon Setup")]
    public Collider weaponCollider; // Drag the BoxCollider of 'GoblinClub' here!
    public float attackCooldown = 2.0f; // Time between club swings
    private float nextAttackTime = 0f;

    [Header("Stats")]
    public int maxHealth = 30;
    public float aggroRadius = 8f;
    public float attackRange = 2f;
    public float moveSpeed = 3.5f;

    [Header("Loot Drop Hook")]
    public GameObject worldItemPrefab; // Drag your WorldItem.prefab here!
    public int guaranteedLootID = 2;   // ID of the potion we made earlier

    // Networked variables guarantee late-joiners see the correct HP and animation state
    public NetworkVariable<int> currentHP = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<AIState> state = new NetworkVariable<AIState>(AIState.Idle, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private NavMeshAgent agent;
    private Transform currentTarget;


    // void Start()
    // {
    //     DontDestroyOnLoad(gameObject);
    // }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            currentHP.Value = maxHealth;
            agent = GetComponent<NavMeshAgent>();
            agent.speed = moveSpeed;
        }
        else
        {
            // LANDMINE #1 DISARMED: We strictly disable the NavMeshAgent on the Clients!
            // If we didn't, the Client's local NavMesh would try to move the monster at the exact 
            // same time the Server's NetworkTransform is trying to pull it, causing severe stuttering.
            if (TryGetComponent(out NavMeshAgent clientAgent)) 
                clientAgent.enabled = false;
        }
    }

    private void Update()
    {
        // // PUT THIS ABOVE THE RETURN:
        // if (Time.frameCount % 60 == 0)
        // {
        //     Debug.Log($"<color=magenta>[HEARTBEAT]</color> Am I Spawned? {IsSpawned} | Am I Server? {IsServer}");
        // }

        // Clients literally do zero thinking. They just read 'state.Value' to play animations.
        if (!IsServer || state.Value == AIState.Dead) return;

        switch (state.Value)
        {
            case AIState.Idle:
                SearchForTarget();
                break;

            case AIState.Chasing:
                ChaseTarget();
                break;

            case AIState.Attacking:
                // // For now, he just stands next to you menacingly. We'll script his swing next.
                // if (Vector3.Distance(transform.position, currentTarget.position) > attackRange)
                //     state.Value = AIState.Chasing;
                // break;

                if (Vector3.Distance(transform.position, currentTarget.position) > attackRange)
                {
                    state.Value = AIState.Chasing;
                }
                else if (Time.time >= nextAttackTime)
                {
                    nextAttackTime = Time.time + attackCooldown;
                    PerformMeleeAttack();
                }
                break;
        }
    }

    // private void SearchForTarget()
    // {
    //     // Find any player on the "Player" physics layer inside our aggro bubble
    //     Collider[] players = Physics.OverlapSphere(transform.position, aggroRadius, LayerMask.GetMask("Player"));
        
    //     if (players.Length > 0)
    //     {
    //         currentTarget = players[0].transform; 
    //         state.Value = AIState.Chasing;
    //     }
    // }

    private void SearchForTarget()
    {
        // Test A: Did Unity actually recognize the word "Player"?
        int layerIndex = LayerMask.NameToLayer("Player");
        int layerMask  = LayerMask.GetMask("Player");

        Collider[] players = Physics.OverlapSphere(transform.position, aggroRadius, layerMask);

        // Print the X-Ray report once every 60 frames so we don't blow up your console
        // if (Time.frameCount % 60 == 0)
        // {
        //     Debug.Log($"<color=orange>[AI X-RAY]</color> Layer 'Player' ID: {layerIndex} | Bitmask: {layerMask} | Objects inside sphere: {players.Length}");
        // }

        if (players.Length > 0)
        {
            currentTarget = players[0].transform; 
            state.Value = AIState.Chasing;
        }
    }




    private void ChaseTarget()
    {
        if (currentTarget == null)
        {
            state.Value = AIState.Idle;
            return;
        }

        agent.SetDestination(currentTarget.position);

        if (Vector3.Distance(transform.position, currentTarget.position) <= attackRange)
        {
            agent.ResetPath(); // Slam on the brakes
            state.Value = AIState.Attacking;
        }
    }

    // LANDMINE #2 DISARMED: Look at "(RequireOwnership = false)".
    // By default, Netcode blocks clients from calling RPCs on objects they don't own. 
    // Because the Server owns the monsters, your player's sword would throw a fatal permission error 
    // trying to hurt it unless we explicitly grant global write-access to this specific method!
    // [ServerRpc(RequireOwnership = false)]

    // [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Owner)]
    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    public void TakeDamageServerRpc(int damage)
    {
        if (state.Value == AIState.Dead) return;

        currentHP.Value -= damage;
        Debug.Log($"Goblin took {damage} damage! HP left: {currentHP.Value}");

        if (currentHP.Value <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        state.Value = AIState.Dead;
        
        // Look at this absolute poetry. The inventory system pays out instantly:
        Vector3 lootSpawnPos = transform.position + (Vector3.up * 0.5f);
        GameObject droppedLoot = Instantiate(worldItemPrefab, lootSpawnPos, Quaternion.identity);
        droppedLoot.GetComponent<NetworkObject>().Spawn();
        
        WorldItemInstance lootData = droppedLoot.GetComponent<WorldItemInstance>();
        lootData.netItemID.Value = guaranteedLootID;
        lootData.netQuantity.Value = 1;

        // Destroy the monster across the network
        GetComponent<NetworkObject>().Despawn(true);
    }


    // This draws the physical OverlapSphere inside your Unity Scene View window!
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, aggroRadius);
    }

    private void PerformMeleeAttack()
    {
        // The Server turns the physical wood into a dangerous payload
        StartCoroutine(ActiveMeleeHitbox());

        // Tell the civilian clients to play a sound / jiggle the monster so it looks like an attack!
        BroadcastAttackVisualClientRpc();
    }


    private System.Collections.IEnumerator ActiveMeleeHitbox()
    {
        if (weaponCollider == null) yield break;


        // THE WIND-UP
        float windupDuration = 0.4f; 
        float timer = 0f;

        while (timer < windupDuration)
        {
            if (currentTarget != null)
            {
                // Calculate the exact angle to the player's moving chest
                Vector3 targetDir = (currentTarget.position - transform.position).normalized;
                targetDir.y = 0; // Keep his feet flat on the grass!

                if (targetDir != Vector3.zero)
                {
                    // Slerp his body to face the player step-for-step as they try to circle him
                    Quaternion lookRot = Quaternion.LookRotation(targetDir);
                    transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, 12f * Time.deltaTime);
                }
            }

            timer += Time.deltaTime;
            yield return null; // Pause the function until the next visual frame draws
        }


        // THE COMMITMENT (Body Lunge)

        weaponCollider.enabled = true;
        if (weaponCollider.attachedRigidbody != null) weaponCollider.attachedRigidbody.WakeUp();

        // THE STEP-IN: Shove the NavMeshAgent forward at Mach 1 to catch back-pedaling players
        if (agent.isOnNavMesh)
        {
            agent.velocity = transform.forward * 4.8f; 
        }

        // The sweeping window (Club does damage for a quarter-second)
        yield return new WaitForSeconds(0.25f);

        // THE RECOVERY (The Punish Window)

        weaponCollider.enabled = false;

        // Slam the brakes on the lunge momentum instantly
        if (agent.isOnNavMesh) agent.velocity = Vector3.zero; 

        if (weaponCollider.TryGetComponent(out DamageCollider dmgCollider))
        {
            dmgCollider.ResetHitboxMemory();
        }
    }



    [ClientRpc]
    private void BroadcastAttackVisualClientRpc()
    {
        // Later, when you replace the Capsule with a real 3D Goblin Rig, 
        // you will trigger: animator.SetTrigger("HeavyAttack"); right here!
        Debug.Log("<color=red>[ENEMY]</color> Goblin swung his club!");
    }
}