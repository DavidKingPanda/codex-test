using System;

namespace Game.Components
{
    /// <summary>
    /// Stores current movement state such as running or walking.
    /// </summary>
    [Serializable]
    public struct MovementStateComponent
    {
        public bool IsRunning;
    }
}
