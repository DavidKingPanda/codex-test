using System;

namespace Game.Networking.Messages
{
    /// <summary>
    /// Snapshot of hunger state for an entity.
    /// </summary>
    [Serializable]
    public struct HungerSnapshot
    {
        public int EntityId;
        public float Current;
        public float Max;

        public HungerSnapshot(int entityId, float current, float max)
        {
            EntityId = entityId;
            Current = current;
            Max = max;
        }
    }
}
