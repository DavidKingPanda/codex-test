using System.Text;
using Game.Networking;
using Game.Networking.Messages;
using UnityEngine;

namespace Game.Infrastructure
{
    /// <summary>
    /// Receives position snapshots from the server and applies them to entity visuals.
    /// </summary>
    public class ClientSnapshotReceiver : MonoBehaviour
    {
        private INetworkTransport networkManager;
        private Transform entityPrefab;
        private readonly System.Collections.Generic.Dictionary<int, Transform> _entities = new();

        public void Initialize(INetworkTransport manager, Transform prefab)
        {
            networkManager = manager;
            entityPrefab = prefab;
            networkManager.OnData += OnDataReceived;
        }

        public void RegisterEntity(int entityId, Transform transform)
        {
            _entities[entityId] = transform;
        }

        private void OnDataReceived(int connectionId, byte[] data)
        {
            var json = Encoding.UTF8.GetString(data);

            var message = JsonUtility.FromJson<NetworkMessage>(json);
            if (message.Type != MessageType.PositionSnapshot)
                return;

            var snapshot = JsonUtility.FromJson<PositionSnapshot>(message.Payload);
            if (snapshot.Equals(default(PositionSnapshot)))
                return;

            if (!_entities.TryGetValue(snapshot.EntityId, out var visual))
            {
                if (entityPrefab == null)
                    return;
                visual = Instantiate(entityPrefab);
                _entities[snapshot.EntityId] = visual;
            }
            visual.position = snapshot.Position;
        }

        private void OnDestroy()
        {
            if (networkManager != null)
            {
                networkManager.OnData -= OnDataReceived;
            }
        }
    }
}
