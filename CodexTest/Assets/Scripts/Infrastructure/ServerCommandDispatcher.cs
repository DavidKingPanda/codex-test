using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

using Game.Domain.Commands;
using Game.Domain.ECS;
using Game.Networking;
using Game.Networking.Messages;
using GameEventBus = Game.EventBus.EventBus;

namespace Game.Infrastructure
{
    /// <summary>
    /// Listens for incoming network data and publishes commands to the event bus.
    /// </summary>
    public class ServerCommandDispatcher : IDisposable
    {
        private readonly INetworkTransport _networkManager;
        private readonly GameEventBus _eventBus;
        private readonly Dictionary<int, Entity> _connectionToEntity;
        private readonly Dictionary<MessageType, Action<int, string>> _handlers = new();


        public ServerCommandDispatcher(
            INetworkTransport networkManager,
            GameEventBus eventBus,
            Dictionary<int, Entity> connectionToEntity)
        {
            _networkManager = networkManager;
            _eventBus = eventBus;
            _connectionToEntity = connectionToEntity;

            _networkManager.OnData += OnDataReceived;

            _handlers[MessageType.MoveCommand] = (id, payload) =>
            {
                var cmd = JsonUtility.FromJson<MoveCommand>(payload);
                if (!cmd.Equals(default(MoveCommand)) && _connectionToEntity.TryGetValue(id, out var entity))
                {
                    var validated = new MoveCommand(entity, cmd.Direction, cmd.Speed, cmd.IsRunning);
                    _eventBus.Publish(validated);
                }
            };

            _handlers[MessageType.JumpCommand] = (id, payload) =>
            {
                var cmd = JsonUtility.FromJson<JumpCommand>(payload);
                if (!cmd.Equals(default(JumpCommand)) && _connectionToEntity.TryGetValue(id, out var entity))
                {
                    var validated = new JumpCommand(entity);
                    _eventBus.Publish(validated);
                }
            };

            _handlers[MessageType.Ping] = (id, payload) =>
            {
                var message = new NetworkMessage(MessageType.Ping, payload);
                _networkManager.SendMessage(id, message);
            };
        }

        private void OnDataReceived(int connectionId, byte[] data)
        {
            var json = Encoding.UTF8.GetString(data);
            var message = JsonUtility.FromJson<NetworkMessage>(json);
            if (_handlers.TryGetValue(message.Type, out var handler))
            {
                handler(connectionId, message.Payload);
            }
        }

        public void Dispose()
        {
            _networkManager.OnData -= OnDataReceived;
        }
    }
}

