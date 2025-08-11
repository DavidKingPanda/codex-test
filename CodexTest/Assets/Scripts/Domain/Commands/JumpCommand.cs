using System;
using Game.Domain.ECS;

namespace Game.Domain.Commands
{
    /// <summary>
    /// Command sent by client to request an entity jump.
    /// Force represents the initial upward velocity applied.
    /// </summary>
    [Serializable]
    public readonly struct JumpCommand
    {
        public readonly Entity Entity;
        public readonly float Force;
        public readonly int ClientId;

        public JumpCommand(Entity entity, float force)
        {
            Entity = entity;
            Force = force;
            ClientId = 0;
        }

        public JumpCommand(int clientId, Entity entity, float force)
        {
            ClientId = clientId;
            Entity = entity;
            Force = force;
        }
    }
}
