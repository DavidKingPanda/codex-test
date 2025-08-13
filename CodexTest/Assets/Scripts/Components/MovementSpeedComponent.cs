using System;

namespace Game.Components
{
    /// <summary>
    /// Holds walk and run speeds for an entity.
    /// </summary>
    [Serializable]
    public struct MovementSpeedComponent
    {
        public float WalkSpeed;
        public float RunSpeed;
    }
}
