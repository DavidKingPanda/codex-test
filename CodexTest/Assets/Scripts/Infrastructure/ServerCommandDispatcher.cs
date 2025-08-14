using System;
using System.Collections.Generic;
using System.Text;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;

using Game.Domain.Commands;
using Game.Domain.ECS;
using Game.Networking;
using Game.Networking.Messages;
using Game.EventBus;

namespace Game.Infrastructure
{
    /// <summary>
    /// Listens for incoming network data and publishes commands to the event bus.
    /// </summary>
    public class ServerCommandDispatcher : IDisposable
    {
        private readonly NetworkManager _networkManager;
        private readonly EventBus _eventBus;
        private readonly Dictionary<NetworkConnection, Entity> _connectionToEntity;
        private readonly Dictionary<MessageType, Action<NetworkConnection, string>> _handlers = new();


        public ServerCommandDispatcher(
            NetworkManager networkManager,
            EventBus eventBus,
            Dictionary<NetworkConnection, Entity> connectionToEntity)
        {
            _networkManager = networkManager;
            _eventBus = eventBus;
            _connectionToEntity = connectionToEntity;

            _networkManager.OnData += OnDataReceived;

            _handlers[MessageType.MoveCommand] = (conn, payload) =>
            {
                var cmd = JsonUtility.FromJson<MoveCommand>(payload);
                if (!cmd.Equals(default(MoveCommand)) && _connectionToEntity.TryGetValue(conn, out var entity))
                {
                    var validated = new MoveCommand(entity, cmd.Direction, cmd.Speed, cmd.IsRunning);
                    _eventBus.Publish(validated);
                }
            };

            _handlers[MessageType.JumpCommand] = (conn, payload) =>
            {
                var cmd = JsonUtility.FromJson<JumpCommand>(payload);
                if (!cmd.Equals(default(JumpCommand)) && _connectionToEntity.TryGetValue(conn, out var entity))
                {
                    var validated = new JumpCommand(entity);
                    _eventBus.Publish(validated);
                }
            };

            _handlers[MessageType.Ping] = (conn, payload) =>
            {
                var message = new NetworkMessage(MessageType.Ping, payload);
                _networkManager.SendMessage(conn, message);
            };
        }

        private void OnDataReceived(NetworkConnection connection, DataStreamReader stream)
        {
            using var bytes = new NativeArray<byte>(stream.Length, Allocator.Temp);
            stream.ReadBytes(bytes);
            var json = Encoding.UTF8.GetString(bytes.ToArray());
            var message = JsonUtility.FromJson<NetworkMessage>(json);
            if (_handlers.TryGetValue(message.Type, out var handler))
            {
                handler(connection, message.Payload);
            }
        }

        public void Dispose()
        {
            _networkManager.OnData -= OnDataReceived;
        }
    }
}

