using System;

namespace Game.Networking.Messages
{
    /// <summary>
    /// Snapshot of stamina state for an entity.
    /// </summary>
    [Serializable]
    public struct StaminaSnapshot
    {
        public int EntityId;
        public float Current;
        public float Max;

        public StaminaSnapshot(int entityId, float current, float max)
        {
            EntityId = entityId;
            Current = current;
            Max = max;
        }
    }
}
