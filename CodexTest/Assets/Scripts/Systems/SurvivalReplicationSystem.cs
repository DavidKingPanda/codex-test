using Game.Domain.ECS;
using Game.Domain.Events;
using Game.Networking;
using Game.Networking.Messages;
using GameEventBus = Game.EventBus.EventBus;
using UnityEngine;

namespace Game.Systems
{
    /// <summary>
    /// Replicates survival stats (stamina and hunger) to clients.
    /// </summary>
    public class SurvivalReplicationSystem : ISystem
    {
        private readonly INetworkTransport _transport;
        private readonly GameEventBus _eventBus;

        public SurvivalReplicationSystem(INetworkTransport transport, GameEventBus eventBus)
        {
            _transport = transport;
            _eventBus = eventBus;
            _eventBus.Subscribe<StaminaChangedEvent>(OnStaminaChanged);
            _eventBus.Subscribe<HungerChangedEvent>(OnHungerChanged);
        }

        public void Update(World world, float deltaTime)
        {
            // Replication is event-driven; nothing per-frame.
        }

        private void OnStaminaChanged(StaminaChangedEvent evt)
        {
            var snapshot = new StaminaSnapshot(evt.Entity.Id, evt.Current, evt.Max);
            var payload = JsonUtility.ToJson(snapshot);
            var message = new NetworkMessage(MessageType.StaminaSnapshot, payload);
            _transport.SendMessage(message);
        }

        private void OnHungerChanged(HungerChangedEvent evt)
        {
            var snapshot = new HungerSnapshot(evt.Entity.Id, evt.Current, evt.Max);
            var payload = JsonUtility.ToJson(snapshot);
            var message = new NetworkMessage(MessageType.HungerSnapshot, payload);
            _transport.SendMessage(message);
        }
    }
}
