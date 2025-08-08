using Game.Domain.ECS;
using Game.Domain.Events;
using Game.Networking;
using Game.Networking.Messages;
using Game.Infrastructure;

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
            _networkManager.SendMessage(snapshot);
        }
    }
}
