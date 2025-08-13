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
    public struct MoveCommand
    {
        public Entity Entity;
        public Vector3 Direction;
        /// <summary>
        /// Movement speed in units per second.
        /// </summary>
        public float Speed;
        /// <summary>
        /// True if the player intends to run.
        /// </summary>
        public bool IsRunning;
        public int ClientId;

        public MoveCommand(Entity entity, Vector3 direction, float speed, bool isRunning)
        {
            Entity = entity;
            Direction = direction;
            Speed = speed;
            IsRunning = isRunning;
            ClientId = 0;
        }

        public MoveCommand(int clientId, Entity entity, Vector3 direction, float speed, bool isRunning)
        {
            ClientId = clientId;
            Entity = entity;
            Direction = direction;
            Speed = speed;
            IsRunning = isRunning;
        }
    }
}
