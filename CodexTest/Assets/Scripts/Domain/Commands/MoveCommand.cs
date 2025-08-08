using UnityEngine;
using Game.Domain.ECS;

namespace Game.Domain.Commands
{
    /// <summary>
    /// Command sent by client to request entity movement.
    /// Direction is the desired movement vector and Speed is expressed
    /// in units per second.
    /// </summary>
    public readonly struct MoveCommand
    {
        public readonly Entity Entity;
        public readonly Vector3 Direction;
        /// <summary>
        /// Movement speed in units per second.
        /// </summary>
        public readonly float Speed;

        public MoveCommand(Entity entity, Vector3 direction, float speed)
        {
            Entity = entity;
            Direction = direction;
            Speed = speed;
        }
    }
}
