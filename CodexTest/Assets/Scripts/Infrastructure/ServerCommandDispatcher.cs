using System;
using System.Collections.Generic;
using System.Text;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;

using Game.Domain.Commands;
using Game.Networking;
using Game.Networking.Messages;

namespace Game.Infrastructure
{
    /// <summary>
    /// Listens for incoming network data and publishes commands to the event bus.
    /// </summary>
    public class ServerCommandDispatcher : IDisposable
    {
        private readonly NetworkManager _networkManager;
        private readonly EventBus _eventBus;
        private readonly Dictionary<MessageType, Action<int, string>> _handlers = new();

        public ServerCommandDispatcher(NetworkManager networkManager, EventBus eventBus)
        {
            _networkManager = networkManager;
            _eventBus = eventBus;
            _networkManager.OnData += OnDataReceived;
            _handlers[MessageType.MoveCommand] = (id, payload) =>
            {
                var cmd = JsonUtility.FromJson<MoveCommand>(payload);
                if (!cmd.Equals(default(MoveCommand)))
                {
                    var serverCmd = new MoveCommand(id, cmd.Entity, cmd.Direction, cmd.Speed);
                    _eventBus.Publish(serverCmd);
                }
            };
            _handlers[MessageType.JumpCommand] = (id, payload) =>
            {
                var cmd = JsonUtility.FromJson<JumpCommand>(payload);
                if (!cmd.Equals(default(JumpCommand)))
                {
                    var serverCmd = new JumpCommand(id, cmd.Entity, cmd.Force);
                    _eventBus.Publish(serverCmd);
                }
            };
        }

        private void OnDataReceived(int clientId, DataStreamReader stream)
        {
            using var bytes = new NativeArray<byte>(stream.Length, Allocator.Temp);
            stream.ReadBytes(bytes);
            var json = Encoding.UTF8.GetString(bytes.ToArray());
            var message = JsonUtility.FromJson<NetworkMessage>(json);
            if (_handlers.TryGetValue(message.Type, out var handler))
            {
                handler(clientId, message.Payload);
            }
        }

        public void Dispose()
        {
            _networkManager.OnData -= OnDataReceived;
        }
    }
}
