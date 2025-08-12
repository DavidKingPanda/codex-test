using System;
using Game.Domain.ECS;

namespace Game.Networking.Messages
{
    /// <summary>
    /// Message sent from server to client to inform about the spawned player entity.
    /// </summary>
    [Serializable]
    public struct SpawnPlayer
    {
        public int EntityId;

        public SpawnPlayer(Entity entity)
        {
            EntityId = entity.Id;
        }
    }
}
