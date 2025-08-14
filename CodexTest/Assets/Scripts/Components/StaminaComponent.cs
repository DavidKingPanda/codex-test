using System;
using Game.Domain.ECS;

namespace Game.Components
{
    /// <summary>
    /// Represents stamina used for sprinting.
    /// </summary>
    [Serializable]
    public struct StaminaComponent : IComponent
    {
        public float Current;
        public float Max;
        /// <summary>
        /// Stamina consumed per second while running.
        /// </summary>
        public float DrainPerSecond;
        /// <summary>
        /// Stamina regenerated per second when not running and not starving.
        /// </summary>
        public float RegenPerSecond;
    }
}
