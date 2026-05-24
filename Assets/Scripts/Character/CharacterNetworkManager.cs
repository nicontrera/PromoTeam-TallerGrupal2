using UnityEngine;
using Unity.Netcode;

namespace NC
{
    public class CharacterNetworkManager : NetworkBehaviour
    {
        protected virtual void Awake()
        {
        }

        [Header("Resources")]
        public NetworkVariable<float> currentStamina = new NetworkVariable<float>(0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<int> maxStamina = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<float> currentHealth = new NetworkVariable<float>(0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<int> maxHealth = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        [Header("Stats")]
        public NetworkVariable<int> endurance = new NetworkVariable<int>(25, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<int> vitality = new NetworkVariable<int>(20, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        // [Header("Level and Range")]
        // public NetworkVariable<int> playerLevel = new NetworkVariable<int>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        // // public NetworkVariable<string> playerRange = new NetworkVariable<string>("knight", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        // public NetworkVariable<int> playerExp = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        // public NetworkVariable<int> expRequiredForNextLevel = new NetworkVariable<int>(50, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);


    }
}
