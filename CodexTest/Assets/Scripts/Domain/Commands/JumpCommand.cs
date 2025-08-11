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

        public JumpCommand(Entity entity, float force)
        {
            Entity = entity;
            Force = force;
        }
    }
}
