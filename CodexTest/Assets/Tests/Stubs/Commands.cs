using System;
using UnityEngine;
using Game.Domain.ECS;
using Game.Infrastructure;

namespace Game.Domain.Commands
{
    /// <summary>Command requesting entity movement.</summary>
    [Serializable]
    public struct MoveCommand
    {
        public Entity Entity;
        public Vector3 Direction;
        public float Speed;
        public bool IsRunning;
        public MoveCommand(Entity entity, Vector3 direction, float speed, bool isRunning)
        {
            Entity = entity;
            Direction = direction;
            Speed = speed;
            IsRunning = isRunning;
        }
    }

    /// <summary>Command requesting entity jump.</summary>
    [Serializable]
    public struct JumpCommand
    {
        public Entity Entity;
        public JumpCommand(Entity entity)
        {
            Entity = entity;
        }
    }
}

namespace Game.Domain
{
    /// <summary>Dispatches commands via the EventBus.</summary>
    public class CommandHandler
    {
        private readonly EventBus _eventBus;
        public CommandHandler(EventBus eventBus) => _eventBus = eventBus;

        public void Handle(Game.Domain.Commands.MoveCommand command) => _eventBus.Publish(command);
        public void Handle(Game.Domain.Commands.JumpCommand command) => _eventBus.Publish(command);
    }
}
