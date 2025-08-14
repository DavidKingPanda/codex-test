using System.Collections.Generic;
using Game.Components;
using Game.Domain.Commands;
using Game.Domain.ECS;
using Game.Domain.Events;
using GameEventBus = Game.EventBus.EventBus;
using Game.Systems;
using NUnit.Framework;
using UnityEngine;

namespace Tests
{
    public class JumpSystemTests
    {
        [Test]
        public void JumpCommandAddsVerticalVelocity()
        {
            var world = new World();
            var eventBus = new GameEventBus();
            var system = new JumpSystem(world, eventBus);
            var entity = world.CreateEntity();
            world.AddComponent(entity, new PositionComponent { Value = Vector3.zero });
            world.AddComponent(entity, new JumpSettingsComponent { JumpForce = 5f, Gravity = -9.81f });

            eventBus.Publish(new JumpCommand(entity));

            Assert.IsTrue(world.TryGetComponent(entity, out VerticalVelocityComponent vel));
            Assert.AreEqual(5f, vel.Value, 1e-5f);
        }

        [Test]
        public void UpdateIntegratesGravityAndLanding()
        {
            var world = new World();
            var eventBus = new GameEventBus();
            var system = new JumpSystem(world, eventBus);
            var entity = world.CreateEntity();
            world.AddComponent(entity, new PositionComponent { Value = Vector3.zero });
            world.AddComponent(entity, new JumpSettingsComponent { JumpForce = 10f, Gravity = -9.8f });
            eventBus.Publish(new JumpCommand(entity));

            var events = new List<PositionChangedEvent>();
            eventBus.Subscribe<PositionChangedEvent>(e => events.Add(e));

            system.Update(world, 0.5f);

            Assert.IsTrue(world.TryGetComponent(entity, out PositionComponent pos1));
            Assert.IsTrue(world.TryGetComponent(entity, out VerticalVelocityComponent vel1));
            Assert.AreEqual(2.55f, pos1.Value.y, 1e-2f);
            Assert.AreEqual(5.1f, vel1.Value, 1e-2f);
            Assert.AreEqual(1, events.Count);
            Assert.AreEqual(2.55f, events[0].Position.y, 1e-2f);

            system.Update(world, 1f);

            Assert.IsTrue(world.TryGetComponent(entity, out PositionComponent pos2));
            Assert.IsTrue(world.TryGetComponent(entity, out VerticalVelocityComponent vel2));
            Assert.AreEqual(0f, pos2.Value.y, 1e-5f);
            Assert.AreEqual(0f, vel2.Value, 1e-5f);
            Assert.AreEqual(2, events.Count);
            Assert.AreEqual(0f, events[1].Position.y, 1e-5f);
        }
    }
}
