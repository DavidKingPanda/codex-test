using System;
using UnityEngine;
using Game.Domain.ECS;

namespace Game.Domain.Commands
{
    /// <summary>
    /// Command sent by client to request entity movement.
    /// Direction is the desired movement vector and Speed is expressed
    /// in units per second.
    /// </summary>
    [Serializable]
    public readonly struct MoveCommand
    {
        public readonly Entity Entity;
        public readonly Vector3 Direction;
        /// <summary>
        /// Movement speed in units per second.
        /// </summary>
        public readonly float Speed;
        public readonly int ClientId;

        public MoveCommand(Entity entity, Vector3 direction, float speed)
        {
            Entity = entity;
            Direction = direction;
            Speed = speed;
            ClientId = 0;
        }

        public MoveCommand(int clientId, Entity entity, Vector3 direction, float speed)
        {
            ClientId = clientId;
            Entity = entity;
            Direction = direction;
            Speed = speed;
        }
    }
}
