using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;

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

        private void OnTriggerEnter(Collider other)
        {
            // if (other.gameObject.layer == LayerMask.NameToLayer("Character"))
            // {
                
            // }
            CharacterManager damageTarget = other.GetComponent<CharacterManager>();

            if (damageTarget != null)
            {
                contactPoint = other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);

                // Check if we can damage this target based on friendly fire

                // Check if target is blocking

                // Check if target is invulnerable

                // Damage our target
                DamageTarget(damageTarget);
            }
        }

        protected virtual void DamageTarget(CharacterManager damageTarget)
        {
            // Dont want to damage the same target more than once in the same attack
            // So add them to a list that checks before applying damage

            if (charactersDamaged.Contains(damageTarget))
                return;

            charactersDamaged.Add(damageTarget);

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
    }
}
