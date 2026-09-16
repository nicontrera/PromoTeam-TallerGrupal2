using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;
using NC;

[System.Serializable]
public class LootDrop
{
    public int itemID;
    public int quantity = 1;
}

public class EnemyAI : CharacterManager
{
    public enum AIState { Idle, Chasing, Attacking, Dead }

    [Header("Melee Weapon Setup")]
    public Collider weaponCollider;
    public float attackCooldown = 2.0f; // Time between club swings
    private float nextAttackTime = 0f;

    [Header("Stats")]
    public int maxHealth = 30;
    public float aggroRadius = 8f;
    public float attackRange = 2f;
    public float moveSpeed = 3.5f;

    [Header("Loot Drop Hook")]
    public GameObject worldItemPrefab;
    public List<LootDrop> lootTable = new List<LootDrop>();

    // Networked variables guarantee late-joiners see the correct HP and animation state
    public NetworkVariable<AIState> state = new NetworkVariable<AIState>(AIState.Idle, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private NavMeshAgent agent;
    private Transform currentTarget;


    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsServer)
        {
            // HEALTH NOW LIVES ON THE SHARED characterNetworkManager, SAME AS PLAYERS -
            // THIS IS WHAT LETS TakeDamageEffect WORK AGAINST EITHER ONE
            characterNetworkManager.maxHealth.Value = maxHealth;
            characterNetworkManager.currentHealth.Value = maxHealth;

            agent = GetComponent<NavMeshAgent>();
            agent.speed = moveSpeed;
        }
        else
        {
            if (TryGetComponent(out NavMeshAgent clientAgent)) 
                clientAgent.enabled = false;
        }
    }

    protected override void Update()
    {
        base.Update();

        // Clients just read 'state.Value' to play animations
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
                // // For now, he just stands next to you menacingly

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
        int layerIndex = LayerMask.NameToLayer("Player");
        int layerMask  = LayerMask.GetMask("Player");

        Collider[] players = Physics.OverlapSphere(transform.position, aggroRadius, layerMask);

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


    // [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Owner)]
    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    public void TakeDamageServerRpc(int damage)
    {
        if (state.Value == AIState.Dead) return;

        // UNIFIED DAMAGE ENTRY POINT: same InstantCharacterEffect pipeline the player uses.
        // Any future damage source (a skill, a spell, a trap) just needs to build one of these
        // and hand it to characterEffectsManager - this works for players AND enemies alike.
        TakeDamageEffect damageEffect = Instantiate(WorldCharacterEffectsManager.instance.takeDamageEffect);
        damageEffect.physicalDamage = damage;
        characterEffectsManager.ProcessInstantEffect(damageEffect);

        Debug.Log($"Goblin took {damage} damage! HP left: {characterNetworkManager.currentHealth.Value}");

        if (characterNetworkManager.currentHealth.Value <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        state.Value = AIState.Dead;

        // Pays out every entry in the loot table, not just one item
        foreach (LootDrop drop in lootTable)
        {
            SpawnLootDrop(drop.itemID, drop.quantity);
        }

        // Destroy the monster across the network
        // GetComponent<NetworkObject>().Despawn(true);
    }

    private void SpawnLootDrop(int itemID, int quantity)
    {
        if (quantity <= 0) return;

        // Small random offset so multiple drops don't all land stacked in the exact same spot
        Vector3 randomOffset = new Vector3(Random.Range(-0.4f, 0.4f), 0f, Random.Range(-0.4f, 0.4f));
        Vector3 lootSpawnPos = transform.position + (Vector3.up * 0.5f) + randomOffset;

        GameObject droppedLoot = Instantiate(worldItemPrefab, lootSpawnPos, Quaternion.identity);
        droppedLoot.GetComponent<NetworkObject>().Spawn();

        WorldItemInstance lootData = droppedLoot.GetComponent<WorldItemInstance>();
        lootData.netItemID.Value = itemID;
        lootData.netQuantity.Value = quantity;
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