using Game.Components;
using Game.Domain.Commands;
using Game.Domain.ECS;
using Game.Domain.Events;
using Game.Infrastructure;
using UnityEngine;

namespace Game.Systems
{
    /// <summary>
    /// Processes MoveCommand events and updates entity positions on the server.
    /// </summary>
    public class MovementSystem : ISystem
    {
        private readonly World _world;
        private readonly EventBus _eventBus;
        private float _fixedDeltaTime;

        public MovementSystem(World world, EventBus eventBus)
        {
            _world = world;
            _eventBus = eventBus;
            _eventBus.Subscribe<MoveCommand>(OnMoveCommand);
        }

        public void Update(World world, float deltaTime)
        {
            // Store the fixed server delta time so MoveCommand processing
            // can apply speed consistently each tick.
            _fixedDeltaTime = deltaTime;
        }

        private void OnMoveCommand(MoveCommand command)
        {
            if (!_world.TryGetComponent(command.Entity, out PositionComponent position))
                return;

            // Determine movement mode based on stamina and desired state.
            float speed = command.Speed;
            bool isRunning = command.IsRunning;

            if (_world.TryGetComponent(command.Entity, out StaminaComponent stamina))
            {
                if (isRunning && stamina.Current <= 0f)
                {
                    isRunning = false;
                }

                // Use configured speeds instead of trusting client supplied speed.
                if (_world.TryGetComponent(command.Entity, out MovementSpeedComponent moveSpeed))
                {
                    speed = isRunning ? moveSpeed.RunSpeed : moveSpeed.WalkSpeed;
                }
            }

            // Physics-based movement with simple collision check.
            Vector3 direction = command.Direction.normalized;
            float distance = speed * _fixedDeltaTime;
            Vector3 target = position.Value + direction * distance;
            if (!Physics.CheckSphere(target, 0.4f))
            {
                position.Value = target;
            }
            _world.SetComponent(command.Entity, position);
            _eventBus.Publish(new PositionChangedEvent(command.Entity, position.Value));

            // Update movement state component
            var state = new MovementStateComponent { IsRunning = isRunning };
            _world.SetComponent(command.Entity, state);
        }
    }
}
