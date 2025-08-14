using Game.Components;
using Game.Domain.ECS;
using Game.Domain.Events;
using Game.EventBus;
using System.Linq;
using UnityEngine;

namespace Game.Systems
{
    /// <summary>
    /// Handles stamina regeneration and hunger depletion.
    /// </summary>
    public class SurvivalSystem : ISystem
    {
        private readonly World _world;
        private readonly EventBus _eventBus;
        private float _fixedDeltaTime;

        public SurvivalSystem(World world, EventBus eventBus)
        {
            _world = world;
            _eventBus = eventBus;
        }

        public void Update(World world, float deltaTime)
        {
            _fixedDeltaTime = deltaTime;

            var staminaEntities = _world.View<StaminaComponent>().ToList();
            foreach (var (entity, stamina) in staminaEntities)
            {
                var st = stamina;
                bool isRunning = false;
                MovementStateComponent movementState;
                if (_world.TryGetComponent(entity, out movementState))
                {
                    isRunning = movementState.IsRunning;
                }

                if (isRunning && st.Current > 0f)
                {
                    st.Current = Mathf.Max(0f, st.Current - st.DrainPerSecond * _fixedDeltaTime);
                }
                else if (st.Current <= 0f && isRunning)
                {
                    // Force entity to stop running when stamina is depleted.
                    if (_world.TryGetComponent(entity, out movementState))
                    {
                        movementState.IsRunning = false;
                        _world.SetComponent(entity, movementState);
                    }
                }
                else if (_world.TryGetComponent(entity, out HungerComponent hunger) && hunger.Current > 0f)
                {
                    st.Current = Mathf.Min(st.Max, st.Current + st.RegenPerSecond * _fixedDeltaTime);
                }

                _world.SetComponent(entity, st);
                _eventBus.Publish(new StaminaChangedEvent(entity, st.Current, st.Max));
            }

            var hungerEntities = _world.View<HungerComponent>().ToList();
            foreach (var (entity, hunger) in hungerEntities)
            {
                var h = hunger;
                h.Current = Mathf.Max(0f, h.Current - h.DrainPerSecond * _fixedDeltaTime);
                _world.SetComponent(entity, h);
                _eventBus.Publish(new HungerChangedEvent(entity, h.Current, h.Max));
            }
        }
    }
}
