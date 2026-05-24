using Unity.Collections;
using Unity.Netcode;

namespace NC
{
    public class EnemyNetworkManager : CharacterNetworkManager
    {
        public EnemyManager enemyManager;
        public NetworkVariable<FixedString64Bytes> enemyName = new NetworkVariable<FixedString64Bytes>("4thEnemy", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        public NetworkVariable<int> currentEnemyHealth = new NetworkVariable<int>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        protected override void Awake() {
            base.Awake();

            enemyManager = GetComponent<EnemyManager>();
        }

        // void Start()
        // {
        //     maxHealth.Value = enemyManager.enemyStatsManager.CalculateHealthBasedOnVitalityLevel(vitality.Value);
        //     currentHealth.Value = maxHealth.Value;
        // }

        public void SetNewMaxHealthValue(int oldVitality, int newVitality)
        {
            maxHealth.Value = enemyManager.enemyStatsManager.CalculateHealthBasedOnVitalityLevel(newVitality);
            currentHealth.Value = maxHealth.Value;
        }

        public void SetNewMaxStaminaValue(int oldEndurance, int newEndurance)
        {
            // THIS ALSO DOES UPDATES CURRENT TO MAX, LIKE WHEN LEVELING UP!
            maxStamina.Value = enemyManager.enemyStatsManager.CalculateStaminaBasedOnEnduranceLevel(newEndurance);
            currentStamina.Value = maxStamina.Value;
        }
    }
}
