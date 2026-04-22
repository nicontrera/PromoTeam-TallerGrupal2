using UnityEngine;

namespace NC
{
    public class EnemyHealth : MonoBehaviour
    {
        public int maxHealth = 100;
        int currentHealth;

        void Start()
        {
            currentHealth = maxHealth;
        }

        public void TakeDamage(int damage)
        {
            currentHealth -= damage;
            Debug.Log("Vida del enemigo: " + currentHealth);

            // Animación de dolor (opcional)
            // animator.SetTrigger("Hurt");

            if (currentHealth <= 0)
            {
                Die();
            }
        }

        void Die()
        {
            Debug.Log("El enemigo murió");
            // Desactivar enemigo o destruir objeto
            Destroy(gameObject);
        }
    }

}
