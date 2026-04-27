
namespace NC
{
    public class PlayerStatsManager : CharacterStatsManager
    {
        PlayerManager player;
        protected override void Awake()
        {
            base.Awake();
            player = GetComponent<PlayerManager>();
        }

        protected override void Start()
        {
            base.Start();
            // WHEN WE MAKE A NEW CHARACTER, AND SET STATS DEPENDING ON CLASS OR SOMETHING, THEN WE CALCULATE INITIAL HEALTH AND ENDURANCE VALUES
            // UNTIL THEN HOWEVER, STATS ARE NEVER CALCULATED, SO WE DO IT HERE ON START
            // IF A SAVE FILE EXISTS THEY WILL BE OVER WRITTEN WHEN LOADING INTO A SCENE
            CalculateHealthBasedOnVitalityLevel(player.playerNetworkManager.vitality.Value);
            CalculateStaminaBasedOnEnduranceLevel(player.playerNetworkManager.endurance.Value);
        }
    }
}
