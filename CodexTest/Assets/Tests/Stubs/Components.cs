using Game.Domain.ECS;
using UnityEngine;

namespace Game.Components
{
    /// <summary>Holds world position of an entity.</summary>
    public struct PositionComponent : IComponent
    {
        public Vector3 Value;
    }

    /// <summary>Walk and run speeds for an entity.</summary>
    public struct MovementSpeedComponent : IComponent
    {
        public float WalkSpeed;
        public float RunSpeed;
    }

    /// <summary>Represents stamina for sprinting.</summary>
    public struct StaminaComponent : IComponent
    {
        public float Current;
        public float Max;
    }

    /// <summary>Current movement state such as running or walking.</summary>
    public struct MovementStateComponent : IComponent
    {
        public bool IsRunning;
    }
}
