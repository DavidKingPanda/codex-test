using System;
using UnityEngine;
using Game.Domain.ECS;

namespace Game.Networking.Messages
{
    /// <summary>
    /// Snapshot message sent from server to clients with entity position.
    /// </summary>
    [Serializable]
    public readonly struct PositionSnapshot
    {
        public readonly int EntityId;
        public readonly Vector3 Position;

        public PositionSnapshot(Entity entity, Vector3 position)
        {
            EntityId = entity.Id;
            Position = position;
        }
    }
}
