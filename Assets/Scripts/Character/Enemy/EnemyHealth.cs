// using Unity.Netcode;
using Unity.Netcode;
using UnityEngine;
// DELETE THIS FILE, CODE IN ENEMYMANAGER.CS
namespace NC
{
    public class EnemyHealth : CharacterManager
    {
        public int maxHealth = 120;
        // public NetworkVariable<FixedString64Bytes> enemyName = new NetworkVariable<FixedString64Bytes>("4thEnemy", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public EnemyNetworkManager enemyNetworkManager;
        public NetworkVariable<int> currentEnemyHealth = new NetworkVariable<int>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        // public PlayerNetworkManager player;
        public PlayerManager playerManager;
        public NetworkObject playerGameObject;
        // public int enemyHP;

        void Start()
        {
            DontDestroyOnLoad(gameObject);
        }

        protected override void Awake()
        {
            base.Awake();
            // DO MORE STUFF, ONLY FOR THE ENEMY CHAR
            enemyNetworkManager = GetComponent<EnemyNetworkManager>();

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
            Debug.Log(enemyNetworkManager.vitality.Value + "and " + enemyNetworkManager.enemyName.Value);
            Debug.Log("enemy health is: " + enemyNetworkManager.currentHealth.Value);

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
                // Die();
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
