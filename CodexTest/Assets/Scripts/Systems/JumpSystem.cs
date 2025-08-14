using Game.Components;
using Game.Domain.Commands;
using Game.Domain.ECS;
using Game.Domain.Events;
using GameEventBus = Game.EventBus.EventBus;
using System.Linq;
using UnityEngine;

namespace Game.Systems
{
    /// <summary>
    /// Applies jump commands and integrates gravity for vertical motion.
    /// </summary>
    public class JumpSystem : ISystem
    {
        private readonly World _world;
        private readonly GameEventBus _eventBus;
        private float _fixedDeltaTime;

        public JumpSystem(World world, GameEventBus eventBus)
        {
            _world = world;
            _eventBus = eventBus;
            _eventBus.Subscribe<JumpCommand>(OnJumpCommand);
        }

        public void Update(World world, float deltaTime)
        {
            _fixedDeltaTime = deltaTime;
            var velocities = _world.View<VerticalVelocityComponent>().ToList();
            foreach (var (entity, vel) in velocities)
            {
                var velocity = vel;
                if (_world.TryGetComponent(entity, out PositionComponent position) &&
                    _world.TryGetComponent(entity, out JumpSettingsComponent jump))
                {
                    velocity.Value += jump.Gravity * _fixedDeltaTime;
                    position.Value.y += velocity.Value * _fixedDeltaTime;
                    if (position.Value.y <= 0f)
                    {
                        position.Value.y = 0f;
                        velocity.Value = 0f;
                    }
                    _world.SetComponent(entity, velocity);
                    _world.SetComponent(entity, position);
                    _eventBus.Publish(new PositionChangedEvent(entity, position.Value));
                }
            }
        }

        private void OnJumpCommand(JumpCommand command)
        {
            if (_world.TryGetComponent(command.Entity, out PositionComponent position) && position.Value.y <= 0f &&
                _world.TryGetComponent(command.Entity, out JumpSettingsComponent jump))
            {
                if (_world.TryGetComponent(command.Entity, out VerticalVelocityComponent velocity))
                {
                    velocity.Value = jump.JumpForce;
                    _world.SetComponent(command.Entity, velocity);
                }
                else
                {
                    _world.AddComponent(command.Entity, new VerticalVelocityComponent { Value = jump.JumpForce });
                }
            }
        }
    }
}
