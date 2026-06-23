using Unity.Netcode;
using UnityEngine;

public class WorldItemInstance : NetworkBehaviour
{
    // Using NetworkVariables guarantees that a player joining 20 minutes late 
    // will automatically receive these numbers and render the right item!
    public NetworkVariable<int> netItemID   = new NetworkVariable<int>(-1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<int> netQuantity = new NetworkVariable<int>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public override void OnNetworkSpawn()
    {
        // If the server changes what this item is, update the local client name/look
        netItemID.OnValueChanged += (oldID, newID) => UpdateAppearance(newID);
        
        // Run it once on Frame 1 for late-joiners
        UpdateAppearance(netItemID.Value);
    }

    private void UpdateAppearance(int itemID)
    {
        if (itemID == -1) return;

        ItemData data = ItemDatabase.GetItemByID(itemID);
        if (data != null)
        {
            // For now, we just change the GameObject's name in the hierarchy so you can see it work.
            // Later, you can have this function spawn a 3D Mesh or floating Sprite!
            gameObject.name = $"LOOT_DROP_{data.itemName}_x{netQuantity.Value}";
        }
    }

    // Called strictly by the Server inside a Player's Pickup RPC
    public void ClaimAndDestroy()
    {
        if (!IsServer) return;
        
        // This is the sacred Netcode call to make an object vanish from the universe for all clients
        GetComponent<NetworkObject>().Despawn(); 
    }
}