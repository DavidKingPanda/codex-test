using UnityEngine;

namespace Game.Infrastructure
{
    /// <summary>
    /// ScriptableObject holding starting survival stats for players.
    /// </summary>
    [CreateAssetMenu(menuName = "Config/SurvivalConfig")]
    public class SurvivalConfig : ScriptableObject
    {
        [Header("Stamina")]
        public float MaxStamina = 100f;
        public float StaminaDrainPerSecond = 10f;
        public float StaminaRegenPerSecond = 5f;

        [Header("Hunger")]
        public float MaxHunger = 100f;
        public float HungerDrainPerSecond = 1f;
    }
}
