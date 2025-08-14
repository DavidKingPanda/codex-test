using System;
using Game.Domain.ECS;

namespace Game.Components
{
    /// <summary>
    /// Stores current movement state such as running or walking.
    /// </summary>
    [Serializable]
    public struct MovementStateComponent : IComponent
    {
        public bool IsRunning;
    }
}
