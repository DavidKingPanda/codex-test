using System;
using Game.Domain.ECS;

namespace Game.Components
{
    /// <summary>
    /// Holds walk and run speeds for an entity.
    /// </summary>
    [Serializable]
    public struct MovementSpeedComponent : IComponent
    {
        public float WalkSpeed;
        public float RunSpeed;
    }
}
