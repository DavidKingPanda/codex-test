using System;
using Game.Domain.ECS;

namespace Game.Components
{
    /// <summary>
    /// Represents hunger decreasing over time.
    /// </summary>
    [Serializable]
    public struct HungerComponent : IComponent
    {
        public float Current;
        public float Max;
        /// <summary>
        /// Hunger drained per second.
        /// </summary>
        public float DrainPerSecond;
    }
}
