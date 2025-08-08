using System.Text;
using System.Text.Json;
using Game.Networking;
using Game.Networking.Messages;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;

namespace Game.Infrastructure
{
    /// <summary>
    /// Receives position snapshots from the server and applies them to the player visual.
    /// </summary>
    public class ClientSnapshotReceiver : MonoBehaviour
    {
        private NetworkManager networkManager;
        private Transform playerVisual;

        public void Initialize(NetworkManager manager, Transform target)
        {
            networkManager = manager;
            playerVisual = target;
            networkManager.OnData += OnDataReceived;
        }

        private void OnDataReceived(DataStreamReader stream)
        {
            var bytes = new NativeArray<byte>(stream.Length, Allocator.Temp);
            stream.ReadBytes(bytes);
            var json = Encoding.UTF8.GetString(bytes.ToArray());
            bytes.Dispose();
            var snapshot = JsonSerializer.Deserialize<PositionSnapshot>(json);
            if (snapshot.EntityId == 0 && playerVisual != null)
            {
                playerVisual.position = snapshot.Position;
            }
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
