using System.Text;
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
            using var bytes = new NativeArray<byte>(stream.Length, Allocator.Temp);
            stream.ReadBytes(bytes);
            var json = Encoding.UTF8.GetString(bytes.ToArray());
            var snapshot = JsonUtility.FromJson<PositionSnapshot>(json);
            if (snapshot != null && playerVisual != null && snapshot.EntityId == 0)
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
