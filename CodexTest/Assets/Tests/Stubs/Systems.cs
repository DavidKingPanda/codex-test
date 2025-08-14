using Game.Components;
using Game.Domain.Commands;
using Game.Domain.ECS;
using Game.Domain.Events;
using Game.Infrastructure;
using Game.Networking;
using Game.Networking.Messages;
using UnityEngine;

namespace Game.Systems
{
    /// <summary>Processes MoveCommand events and updates positions.</summary>
    public class MovementSystem : ISystem
    {
        private readonly World _world;
        private readonly EventBus _eventBus;
        private float _delta;

        public MovementSystem(World world, EventBus eventBus)
        {
            _world = world;
            _eventBus = eventBus;
            _eventBus.Subscribe<MoveCommand>(OnMoveCommand);
        }

        public void Update(World world, float deltaTime)
        {
            _delta = deltaTime;
        }

        private void OnMoveCommand(MoveCommand command)
        {
            if (!_world.TryGetComponent(command.Entity, out PositionComponent pos))
                return;

            bool isRunning = command.IsRunning && command.Direction.sqrMagnitude > 0f;
            if (_world.TryGetComponent(command.Entity, out StaminaComponent stamina) && isRunning && stamina.Current <= 0f)
                isRunning = false;

            float speed = command.Speed;
            if (_world.TryGetComponent(command.Entity, out MovementSpeedComponent moveSpeed))
                speed = isRunning ? moveSpeed.RunSpeed : moveSpeed.WalkSpeed;

            Vector3 dir = command.Direction.normalized;
            pos.Value += dir * speed * _delta;
            _world.SetComponent(command.Entity, pos);
            _world.SetComponent(command.Entity, new MovementStateComponent { IsRunning = isRunning });
            _eventBus.Publish(new PositionChangedEvent(command.Entity, pos.Value));
        }
    }

    /// <summary>Sends snapshot messages when positions change.</summary>
    public class ReplicationSystem : ISystem
    {
        private readonly NetworkManager _networkManager;
        private readonly EventBus _eventBus;

        public ReplicationSystem(NetworkManager networkManager, EventBus eventBus)
        {
            _networkManager = networkManager;
            _eventBus = eventBus;
            _eventBus.Subscribe<PositionChangedEvent>(OnPositionChanged);
        }

        public void Update(World world, float deltaTime) { }

        private void OnPositionChanged(PositionChangedEvent evt)
        {
            var snapshot = new PositionSnapshot(evt.Entity, evt.Position);
            var payload = JsonUtility.ToJson(snapshot);
            var message = new NetworkMessage(MessageType.PositionSnapshot, payload);
            _networkManager.SendMessage(message);
        }
    }
}
