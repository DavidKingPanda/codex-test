using System;
using Game.Domain.ECS;

namespace Game.Networking.Messages
{
    /// <summary>
    /// Message sent from server to client to inform about the spawned player entity.
    /// </summary>
    [Serializable]
    public readonly struct SpawnPlayer
    {
        public readonly int EntityId;

        public SpawnPlayer(Entity entity)
        {
            EntityId = entity.Id;
        }
    }
}
