using UnityEngine;
using Game.Domain.ECS;

namespace Game.Domain.Events
{
    /// <summary>Raised when an entity's position changes.</summary>
    public readonly struct PositionChangedEvent
    {
        public readonly Entity Entity;
        public readonly Vector3 Position;
        public PositionChangedEvent(Entity entity, Vector3 position)
        {
            Entity = entity;
            Position = position;
        }
    }
}
