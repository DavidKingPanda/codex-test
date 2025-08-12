using Game.Domain.ECS;
using Game.Networking;
using Game.Networking.Messages;
using System.Text;
using Unity.Collections;
using Unity.Networking.Transport;
using Game.Presentation;
using UnityEngine;

namespace Game.Infrastructure
{
    /// <summary>
    /// Starts the client network connection and wires up input and camera.
    /// </summary>
    public class ClientBootstrap : MonoBehaviour
    {
        [SerializeField] private string address = "127.0.0.1";
        [SerializeField] private ushort port = 7777;
        [SerializeField] private ClientInputSender inputSender;
        [SerializeField] private Transform playerVisual;
        [SerializeField] private CameraFollow cameraFollow;
        [SerializeField] private ClientSnapshotReceiver snapshotReceiver;

        private NetworkManager networkManager;
        private bool playerInitialized;

        private void Start()
        {
            networkManager = new NetworkManager();
            networkManager.StartClient(address, port);
            networkManager.OnData += OnDataReceived;
        }

        private void OnDataReceived(NetworkConnection connection, DataStreamReader stream)
        {
            using var bytes = new NativeArray<byte>(stream.Length, Allocator.Temp);
            stream.ReadBytes(bytes);
            var json = Encoding.UTF8.GetString(bytes.ToArray());
            var message = JsonUtility.FromJson<NetworkMessage>(json);
            if (message.Type != MessageType.SpawnPlayer || playerInitialized)
                return;

            var spawn = JsonUtility.FromJson<SpawnPlayer>(message.Payload);
            var entity = new Entity(spawn.EntityId);
            if (inputSender != null)
            {
                inputSender.Initialize(networkManager, entity, playerVisual);
            }
            snapshotReceiver.Initialize(networkManager, playerVisual);
            snapshotReceiver.RegisterEntity(entity.Id, playerVisual);
            if (cameraFollow != null && playerVisual != null)
            {
                cameraFollow.SetTarget(playerVisual);
            }

            playerInitialized = true;
        }

        private void Update()
        {
            networkManager.Update();
        }

        private void OnDestroy()
        {
            if (networkManager != null)
            {
                networkManager.OnData -= OnDataReceived;
                networkManager.Dispose();
            }
        }
    }
}
