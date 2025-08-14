using System.Collections.Generic;
using Game.Components;
using Game.Domain.ECS;
using Game.Domain.Events;
using Game.Infrastructure;
using Game.Systems;
using NUnit.Framework;

namespace Tests
{
    public class SurvivalSystemTests
    {
        [Test]
        public void RunningDrainsStaminaAndStopsWhenEmpty()
        {
            var world = new World();
            var eventBus = new EventBus();
            var system = new SurvivalSystem(world, eventBus);
            var entity = world.CreateEntity();
            world.AddComponent(entity, new StaminaComponent { Current = 2f, Max = 5f, DrainPerSecond = 1f, RegenPerSecond = 1f });
            world.AddComponent(entity, new HungerComponent { Current = 5f, Max = 5f, DrainPerSecond = 0f });
            world.AddComponent(entity, new MovementStateComponent { IsRunning = true });

            system.Update(world, 2f);
            system.Update(world, 0.1f);

            Assert.IsTrue(world.TryGetComponent(entity, out StaminaComponent stamina));
            Assert.AreEqual(0f, stamina.Current, 1e-5f);
            Assert.IsTrue(world.TryGetComponent(entity, out MovementStateComponent state));
            Assert.IsFalse(state.IsRunning);
        }

        [Test]
        public void StaminaRegeneratesWhenNotRunning()
        {
            var world = new World();
            var eventBus = new EventBus();
            var system = new SurvivalSystem(world, eventBus);
            var entity = world.CreateEntity();
            world.AddComponent(entity, new StaminaComponent { Current = 0f, Max = 5f, DrainPerSecond = 1f, RegenPerSecond = 2f });
            world.AddComponent(entity, new HungerComponent { Current = 5f, Max = 5f, DrainPerSecond = 0f });
            world.AddComponent(entity, new MovementStateComponent { IsRunning = false });

            system.Update(world, 1f);

            Assert.IsTrue(world.TryGetComponent(entity, out StaminaComponent stamina));
            Assert.AreEqual(2f, stamina.Current, 1e-5f);
        }

        [Test]
        public void HungerDecreasesOverTime()
        {
            var world = new World();
            var eventBus = new EventBus();
            var system = new SurvivalSystem(world, eventBus);
            var entity = world.CreateEntity();
            world.AddComponent(entity, new HungerComponent { Current = 5f, Max = 5f, DrainPerSecond = 1f });

            var events = new List<HungerChangedEvent>();
            eventBus.Subscribe<HungerChangedEvent>(e => events.Add(e));

            system.Update(world, 2f);

            Assert.IsTrue(world.TryGetComponent(entity, out HungerComponent hunger));
            Assert.AreEqual(3f, hunger.Current, 1e-5f);
            Assert.AreEqual(1, events.Count);
            Assert.AreEqual(3f, events[0].Current, 1e-5f);
        }
    }
}
