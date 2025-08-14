using Game.Domain.ECS;
using Game.Domain.Events;
using Game.Networking;
using Game.Networking.Messages;
using EventBus = Game.EventBus.EventBus;
using UnityEngine;

namespace Game.Systems
{
    /// <summary>
    /// Sends position snapshots to clients when entities move.
    /// </summary>
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

        public void Update(World world, float deltaTime)
        {
            // Replication is event-driven; nothing per-frame here.
        }

        private void OnPositionChanged(PositionChangedEvent evt)
        {
            var snapshot = new PositionSnapshot(evt.Entity, evt.Position);
            var payload = JsonUtility.ToJson(snapshot);
            var message = new NetworkMessage(MessageType.PositionSnapshot, payload);
            _networkManager.SendMessage(message);
        }
    }
}
