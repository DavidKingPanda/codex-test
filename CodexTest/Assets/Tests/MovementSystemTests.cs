using System.Collections.Generic;
using Game.Components;
using Game.Domain.Commands;
using Game.Domain.ECS;
using Game.Domain.Events;
using EventBus = Game.EventBus.EventBus;
using Game.Systems;
using NUnit.Framework;
using UnityEngine;

namespace Tests
{
    public class MovementSystemTests
    {
        [Test]
        public void MoveCommandUpdatesPositionAndState()
        {
            var world = new World();
            var eventBus = new EventBus();
            var system = new MovementSystem(world, eventBus);
            var entity = world.CreateEntity();
            world.AddComponent(entity, new PositionComponent { Value = Vector3.zero });
            world.AddComponent(entity, new MovementSpeedComponent { WalkSpeed = 1f, RunSpeed = 2f });
            world.AddComponent(entity, new StaminaComponent { Current = 5f, Max = 5f });

            var events = new List<PositionChangedEvent>();
            eventBus.Subscribe<PositionChangedEvent>(e => events.Add(e));

            system.Update(world, 1f);
            eventBus.Publish(new MoveCommand(entity, Vector3.right, 0f, true));

            Assert.IsTrue(world.TryGetComponent(entity, out PositionComponent pos));
            Assert.AreEqual(2f, pos.Value.x, 1e-5f);
            Assert.AreEqual(1, events.Count);
            Assert.AreEqual(new Vector3(2f, 0f, 0f), events[0].Position);
            Assert.IsTrue(world.TryGetComponent(entity, out MovementStateComponent state));
            Assert.IsTrue(state.IsRunning);
        }

        [Test]
        public void MoveCommandWithoutStaminaFallsBackToWalk()
        {
            var world = new World();
            var eventBus = new EventBus();
            var system = new MovementSystem(world, eventBus);
            var entity = world.CreateEntity();
            world.AddComponent(entity, new PositionComponent { Value = Vector3.zero });
            world.AddComponent(entity, new MovementSpeedComponent { WalkSpeed = 1f, RunSpeed = 2f });
            world.AddComponent(entity, new StaminaComponent { Current = 0f, Max = 5f });

            system.Update(world, 1f);
            eventBus.Publish(new MoveCommand(entity, Vector3.right, 0f, true));

            Assert.IsTrue(world.TryGetComponent(entity, out PositionComponent pos));
            Assert.AreEqual(1f, pos.Value.x, 1e-5f);
            Assert.IsTrue(world.TryGetComponent(entity, out MovementStateComponent state));
            Assert.IsFalse(state.IsRunning);
        }
    }
}

