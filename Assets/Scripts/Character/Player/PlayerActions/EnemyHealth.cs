// using Unity.Netcode;
using Unity.Netcode;
using UnityEngine;

namespace NC
{
    public class EnemyHealth : NetworkBehaviour
    {
        public int maxHealth = 120;
        public NetworkVariable<int> currentEnemyHealth = new NetworkVariable<int>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        // public PlayerNetworkManager player;
        public PlayerManager playerManager;
        public NetworkObject playerGameObject;
        // public int enemyHP;

        void Start()
        {
            DontDestroyOnLoad(gameObject);
        }


        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                currentEnemyHealth.Value = maxHealth;
            }

            // (Optional) Listen for changes so you can update UI or play effects
            Debug.Log("Enemy OnNetworkSpawn call");
            currentEnemyHealth.OnValueChanged += OnHealthChanged;
        }

        private void OnHealthChanged(int previousValue, int newValue)
        {
            Debug.Log($"Enemy HP changed from {previousValue} to {newValue}");
            if (newValue <= 0)
            {
                // Play death animation locally on every client
            }
        }

        public void TakeDamage(int damage, ulong playerId)
        {

            playerGameObject = NetworkManager.Singleton.ConnectedClients[playerId].PlayerObject;

            playerManager  = playerGameObject.GetComponent<PlayerManager>();

            RequestDamageServerRpc(damage, playerId);

            Debug.Log("Player with " + playerId + " id, damaged this enemy");
        }

        [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
        private void RequestDamageServerRpc(int damage, ulong playerId)
        {
            if (!IsServer)
                return;
            currentEnemyHealth.Value -= damage;
            Debug.Log("Vida del enemigo: " + currentEnemyHealth.Value);
            Debug.Log("Attacked by player with id: " + playerId);

            if (currentEnemyHealth.Value <= 0)
            {
                Debug.Log("here was the call to singleton or single instance object");
                Die();
            }
        }

        void Die()
        {
            Debug.Log("The enemy died");

            if (!IsServer) return;

            GetComponent<NetworkObject>().Despawn();
        }

        public override void OnNetworkDespawn()
        {
            currentEnemyHealth.OnValueChanged -= OnHealthChanged;
        }
    }

}
