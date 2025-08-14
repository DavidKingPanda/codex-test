using System.Text;
using Game.Domain.Events;
using Game.Domain.ECS;
using GameEventBus = Game.EventBus.EventBus;
using Game.Networking;
using Game.Networking.Messages;
using UnityEngine;

namespace Game.Infrastructure
{
    /// <summary>
    /// Receives survival stat snapshots from the server and publishes events for the local player.
    /// </summary>
    public class StatsSnapshotReceiver : MonoBehaviour
    {
        private INetworkTransport _transport;
        private GameEventBus _eventBus;
        private int _playerEntityId;

        public void Initialize(INetworkTransport transport, GameEventBus eventBus, Entity player)
        {
            _transport = transport;
            _eventBus = eventBus;
            _playerEntityId = player.Id;
            _transport.OnData += OnDataReceived;
        }

        private void OnDataReceived(int connectionId, byte[] data)
        {
            var json = Encoding.UTF8.GetString(data);
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
            if (_transport != null)
            {
                _transport.OnData -= OnDataReceived;
            }
        }
    }
}
