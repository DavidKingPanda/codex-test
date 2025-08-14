using System;
using Game.Domain.ECS;

namespace Game.Domain.Commands
{
    /// <summary>
    /// Command sent by client to request an entity jump.
    /// </summary>
    [Serializable]
    public struct JumpCommand
    {
        public Entity Entity;
        public int ClientId;

        public JumpCommand(Entity entity)
        {
            Entity = entity;
            ClientId = 0;
        }

        public JumpCommand(int clientId, Entity entity)
        {
            ClientId = clientId;
            Entity = entity;
        }
    }
}
