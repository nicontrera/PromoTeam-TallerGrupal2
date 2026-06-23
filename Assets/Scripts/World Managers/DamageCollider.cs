using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


namespace NC
{
    public class DamageCollider : MonoBehaviour
    {
        [Header("Damage")]
        public float physicalDamage = 0; // Later we can split it into subtypes like, "standard", "slash", "blunt", "pierce", "ranged"
        public float magicDamage = 0;
        public float fireDamage = 0;
        public float iceDamage = 0;
        public float lightMagicDamage = 0;
        public float darkMagicDamage = 0;

        [Header("Contact Point")]
        protected Vector3 contactPoint;

        [Header("Characters Damaged")]
        protected List<CharacterManager> charactersDamaged = new List<CharacterManager>();

        // private void OnTriggerEnter(Collider other)
        // {
        //     // if (other.gameObject.layer == LayerMask.NameToLayer("Character"))
        //     // {
                
        //     // }
        //     CharacterManager damageTarget = other.GetComponent<CharacterManager>();

        //     if (damageTarget != null)
        //     {
        //         contactPoint = other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);

        //         // Check if we can damage this target based on friendly fire

        //         // Check if target is blocking

        //         // Check if target is invulnerable

        //         // Damage our target
        //         DamageTarget(damageTarget);
        //     }
        // }

        private void OnTriggerEnter(Collider other)
        {
            // GOD-TIER SOULS TRICK: If the thing the weapon touched shares our exact same highest-level parent, ignore it instantly!
            if (other.transform.root == transform.root) return;

            // WIRETAP LEVEL 1: Did the wood physically cross ANY object's boundary whatsoever?
            Debug.Log($"<color=cyan>[CLUB WIRETAP]</color> Wood touched collider: '{other.name}' (Layer: {LayerMask.LayerToName(other.gameObject.layer)})");

            // THE FIX: We use GetComponentInParent instead of standard GetComponent! 
            // (If the club hit your player's fingertip bone collider, standard GetComponent returns null).
            CharacterManager damageTarget = other.GetComponentInParent<CharacterManager>();

            if (damageTarget != null)
            {
                Debug.Log($"<color=yellow>[CLUB WIRETAP]</color> Target confirmed as CharacterManager: {damageTarget.gameObject.name}");

                contactPoint = other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);

                DamageTarget(damageTarget);
            }
        }

        // protected virtual void DamageTarget(CharacterManager damageTarget)
        // {
        //     // Dont want to damage the same target more than once in the same attack
        //     // So add them to a list that checks before applying damage

        //     if (charactersDamaged.Contains(damageTarget))
        //         return;

        //     charactersDamaged.Add(damageTarget);

        //     TakeDamageEffect damageEffect = Instantiate(WorldCharacterEffectsManager.instance.takeDamageEffect);
        //     damageEffect.physicalDamage = physicalDamage;
        //     damageEffect.magicDamage = magicDamage;
        //     damageEffect.lightMagicDamage = lightMagicDamage;
        //     damageEffect.darkMagicDamage = darkMagicDamage;
        //     damageEffect.fireDamage = fireDamage;
        //     damageEffect.iceDamage = iceDamage;
        //     damageEffect.contactPoint = contactPoint;

        //     damageTarget.characterEffectsManager.ProcessInstantEffect(damageEffect);
        // }



        protected virtual void DamageTarget(CharacterManager damageTarget)
        {
            // SERVER AUTHORITY ONLY: Civilian computers are legally blind to combat triggers.
            if (!NetworkManager.Singleton.IsServer) return;

            if (charactersDamaged.Contains(damageTarget))
                return;

            charactersDamaged.Add(damageTarget);

            // PATH A: The Server hit an object it already owns (like a Goblin, or the Host Player).
            // We run your original SO logic instantly without paying for internet latency!
            if (damageTarget.IsOwner)
            {
                TakeDamageEffect damageEffect = Instantiate(WorldCharacterEffectsManager.instance.takeDamageEffect);
                damageEffect.physicalDamage = physicalDamage;
                damageEffect.magicDamage = magicDamage;
                damageEffect.lightMagicDamage = lightMagicDamage;
                damageEffect.darkMagicDamage = darkMagicDamage;
                damageEffect.fireDamage = fireDamage;
                damageEffect.iceDamage = iceDamage;
                damageEffect.contactPoint = contactPoint;

                damageTarget.characterEffectsManager.ProcessInstantEffect(damageEffect);
            }
            // PATH B: The Server hit a remote Co-Op Client across the internet. Mail them the Subpoena!
            else
            {
                // Look up the exact internet address (OwnerClientId) of the player who got hit
                ClientRpcParams rpcParams = new ClientRpcParams
                {
                    Send = new ClientRpcSendParams { TargetClientIds = new ulong[] { damageTarget.OwnerClientId } }
                };

                // Transmit the payload across the network directly to their specific PC
                damageTarget.characterNetworkManager.NotifyTakeDamageClientRpc(
                    physicalDamage, magicDamage, fireDamage, iceDamage, lightMagicDamage, darkMagicDamage, 
                    contactPoint, rpcParams
                );
            }
        }

        public void ResetHitboxMemory()
        {
            charactersDamaged.Clear();
        }
    }
}
