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

        // ClientRpcParams allows the Server to whisper this RPC directly into one specific player's ear,
        // rather than broadcasting the packet to the entire 4-player lobby!
        [ClientRpc]
        public void NotifyTakeDamageClientRpc(
            float phys, float magic, float fire, float ice, float light, float dark, Vector3 contact, 
            ClientRpcParams clientRpcParams = default)
        {
            // SECURITY GATE: Only the legal owner of this character executes the ScriptableObject
            if (!IsOwner) return;

            Debug.Log($"<color=red>[CLIENT SUBPOENA]</color> Received damage packet! Executing local effect...");

            // 1. Pull a fresh copy of your master damage template from the global registry
            TakeDamageEffect damageEffect = Instantiate(WorldCharacterEffectsManager.instance.takeDamageEffect);
            
            // 2. Inject the authoritative numbers sent over the internet by the Server
            damageEffect.physicalDamage = phys;
            damageEffect.magicDamage = magic;
            damageEffect.fireDamage = fire;
            damageEffect.iceDamage = ice;
            damageEffect.lightMagicDamage = light;
            damageEffect.darkMagicDamage = dark;
            damageEffect.contactPoint = contact;

            // 3. Hand it to your pre-existing local effects processor!
            CharacterManager character = GetComponent<CharacterManager>();
            character.characterEffectsManager.ProcessInstantEffect(damageEffect);
        }

    }
}
