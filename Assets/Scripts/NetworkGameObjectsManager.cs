using Unity.Netcode;
using UnityEngine;

namespace NC
{
    public class NetworkGameObjectsManager : NetworkBehaviour
    {
        [SerializeField] private GameObject enemyPrefab;

    // Only want to spawn this when the NetworkManager starts up on the Server
        public override void OnNetworkSpawn()
        {
            if (IsServer)
                SpawnEnemy();
        }

        private void SpawnEnemy()
        {
            // Instantiate the prefab locally on the server
            GameObject instance = Instantiate(enemyPrefab);
            NetworkObject networkObject = instance.GetComponent<NetworkObject>();
            // Spawn it across the network so all clients see it
            networkObject.Spawn();
        }
    }
}
