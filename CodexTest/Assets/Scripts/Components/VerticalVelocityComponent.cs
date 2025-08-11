using Game.Domain.ECS;

namespace Game.Components
{
    /// <summary>
    /// Stores vertical velocity for jump and gravity simulation.
    /// Pure data only.
    /// </summary>
    public struct VerticalVelocityComponent : IComponent
    {
        public float Value;
    }
}
