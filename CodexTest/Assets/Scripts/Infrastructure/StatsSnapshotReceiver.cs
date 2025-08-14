using System.Text;
using Game.Domain.Events;
using Game.Domain.ECS;
using Game.Infrastructure;
using Game.Networking;
using Game.Networking.Messages;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;

namespace Game.Infrastructure
{
    /// <summary>
    /// Receives survival stat snapshots from the server and publishes events for the local player.
    /// </summary>
    public class StatsSnapshotReceiver : MonoBehaviour
    {
        private NetworkManager _networkManager;
        private EventBus _eventBus;
        private int _playerEntityId;

        public void Initialize(NetworkManager manager, EventBus eventBus, Entity player)
        {
            _networkManager = manager;
            _eventBus = eventBus;
            _playerEntityId = player.Id;
            _networkManager.OnData += OnDataReceived;
        }

        private void OnDataReceived(NetworkConnection connection, DataStreamReader stream)
        {
            using var bytes = new NativeArray<byte>(stream.Length, Allocator.Temp);
            stream.ReadBytes(bytes);
            var json = Encoding.UTF8.GetString(bytes.ToArray());
            var message = JsonUtility.FromJson<NetworkMessage>(json);

            switch (message.Type)
            {
                case MessageType.StaminaSnapshot:
                    var stamina = JsonUtility.FromJson<StaminaSnapshot>(message.Payload);
                    if (stamina.EntityId == _playerEntityId)
                    {
                        _eventBus.Publish(new StaminaChangedEvent(new Entity(stamina.EntityId), stamina.Current, stamina.Max));
                    }
                    break;
                case MessageType.HungerSnapshot:
                    var hunger = JsonUtility.FromJson<HungerSnapshot>(message.Payload);
                    if (hunger.EntityId == _playerEntityId)
                    {
                        _eventBus.Publish(new HungerChangedEvent(new Entity(hunger.EntityId), hunger.Current, hunger.Max));
                    }
                    break;
                default:
                    return;
            }
        }

        private void OnDestroy()
        {
            if (_networkManager != null)
            {
                _networkManager.OnData -= OnDataReceived;
            }
        }
    }
}
