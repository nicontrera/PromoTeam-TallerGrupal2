
namespace NC
{
    public class EnemyStatsManager : CharacterStatsManager
    {
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        EnemyManager enemyManager;
            protected override void Awake()
            {
                base.Awake();
                enemyManager = GetComponent<EnemyManager>();
            }

            protected override void Start()
            {
                base.Start();
                enemyManager.enemyNetworkManager.maxHealth.Value = CalculateHealthBasedOnVitalityLevel(enemyManager.enemyNetworkManager.vitality.Value);
                enemyManager.enemyNetworkManager.currentHealth.Value = enemyManager.enemyNetworkManager.maxHealth.Value;
            }
    }
}
