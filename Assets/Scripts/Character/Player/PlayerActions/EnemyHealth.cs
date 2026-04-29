using UnityEngine;

namespace NC
{
    public class EnemyHealth : MonoBehaviour
    {
        public WorldSaveGameManager worldSaveGameManager;
        public PlayerManager playerManager;
        public int maxHealth = 40;
        int currentHealth;

        void Start()
        {
            currentHealth = maxHealth;
        }

        public void TakeDamage(int damage)
        {
            currentHealth -= damage;
            Debug.Log("Vida del enemigo: " + currentHealth);

            // animator.SetTrigger("Hurt");

            if (currentHealth <= 0)
            {
                worldSaveGameManager.CheckForLevelUp(30);
                //Die();
            }
        }

        void Die()
        {
            Debug.Log("El enemigo murió");
            Destroy(gameObject);
        }
    }

}
