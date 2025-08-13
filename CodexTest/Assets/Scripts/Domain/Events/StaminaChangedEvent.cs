using Game.Domain.ECS;

namespace Game.Domain.Events
{
    /// <summary>
    /// Raised when an entity's stamina changes.
    /// </summary>
    public struct StaminaChangedEvent
    {
        public Entity Entity;
        public float Current;
        public float Max;

        public StaminaChangedEvent(Entity entity, float current, float max)
        {
            Entity = entity;
            Current = current;
            Max = max;
        }
    }
}
