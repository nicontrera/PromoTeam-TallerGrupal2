using Unity.Netcode;
using UnityEngine;

public class ServerSpawner : NetworkBehaviour
{
    public GameObject monsterPrefab; // Drag your Goblin PREFAB here in the Inspector!

    public override void OnNetworkSpawn()
    {
        // Only the server is legally allowed to birth physical matter
        if (!IsServer) return;

        // GameObject spawnedMonster = Instantiate(monsterPrefab, transform.position, Quaternion.identity);
        
        // // This is the sacred Netcode command that forces the engine to register the object, 
        // // assign it a NetworkInstanceId, and broadcast its existence to all connected PCs:
        // spawnedMonster.GetComponent<NetworkObject>().Spawn();

        GameObject spawnedMonster = Instantiate(monsterPrefab);
        
        // This is the sacred Netcode command that forces the engine to register the object, 
        // assign it a NetworkInstanceId, and broadcast its existence to all connected PCs:
        spawnedMonster.GetComponent<NetworkObject>().Spawn();
    }
}