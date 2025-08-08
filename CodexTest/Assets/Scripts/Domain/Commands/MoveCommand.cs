using System;
using UnityEngine;
using Game.Domain.ECS;

namespace Game.Domain.Commands
{
    /// <summary>
    /// Command sent by client to request entity movement.
    /// </summary>
    [Serializable]
    public readonly struct MoveCommand
    {
        public readonly Entity Entity;
        public readonly Vector3 Direction;
        public readonly float Speed;

        public MoveCommand(Entity entity, Vector3 direction, float speed)
        {
            Entity = entity;
            Direction = direction;
            Speed = speed;
        }
    }
}
