using Game.Domain.ECS;

namespace Game.Domain.Events
{
    /// <summary>
    /// Raised when an entity's hunger changes.
    /// </summary>
    public struct HungerChangedEvent
    {
        public Entity Entity;
        public float Current;
        public float Max;

        public HungerChangedEvent(Entity entity, float current, float max)
        {
            Entity = entity;
            Current = current;
            Max = max;
        }
    }
}
