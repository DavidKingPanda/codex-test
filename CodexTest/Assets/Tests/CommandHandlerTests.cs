using Game.Domain;
using Game.Domain.Commands;
using Game.Domain.ECS;
using GameEventBus = Game.EventBus.EventBus;
using NUnit.Framework;
using UnityEngine;

namespace Tests
{
    public class CommandHandlerTests
    {
        [Test]
        public void HandleMoveCommand_PublishesCommand()
        {
            var eventBus = new GameEventBus();
            var handler = new CommandHandler(eventBus);
            MoveCommand received = default;
            eventBus.Subscribe<MoveCommand>(cmd => received = cmd);

            var command = new MoveCommand(new Entity(1), Vector3.forward, 1f, false);
            handler.Handle(command);

            Assert.AreEqual(command.Entity, received.Entity);
            Assert.AreEqual(command.Direction, received.Direction);
        }

        [Test]
        public void HandleJumpCommand_PublishesCommand()
        {
            var eventBus = new GameEventBus();
            var handler = new CommandHandler(eventBus);
            JumpCommand received = default;
            eventBus.Subscribe<JumpCommand>(cmd => received = cmd);

            var command = new JumpCommand(new Entity(2));
            handler.Handle(command);

            Assert.AreEqual(command.Entity, received.Entity);
        }
    }
}

