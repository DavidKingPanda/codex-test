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
        private readonly Dictionary<MessageType, Action<string>> _handlers = new();

        public ServerCommandDispatcher(NetworkManager networkManager, EventBus eventBus)
        {
            _networkManager = networkManager;
            _eventBus = eventBus;
            _networkManager.OnData += OnDataReceived;
            _handlers[MessageType.MoveCommand] = payload =>
            {
                var cmd = JsonUtility.FromJson<MoveCommand>(payload);
                if (!cmd.Equals(default(MoveCommand)))
                {
                    _eventBus.Publish(cmd);
                }
            };
            _handlers[MessageType.JumpCommand] = payload =>
            {
                var cmd = JsonUtility.FromJson<JumpCommand>(payload);
                if (!cmd.Equals(default(JumpCommand)))
                {
                    _eventBus.Publish(cmd);
                }
            };
        }

        private void OnDataReceived(DataStreamReader stream)
        {
            using var bytes = new NativeArray<byte>(stream.Length, Allocator.Temp);
            stream.ReadBytes(bytes);
            var json = Encoding.UTF8.GetString(bytes.ToArray());
            var message = JsonUtility.FromJson<NetworkMessage>(json);
            if (_handlers.TryGetValue(message.Type, out var handler))
            {
                handler(message.Payload);
            }
        }

        public void Dispose()
        {
            _networkManager.OnData -= OnDataReceived;
        }
    }
}
