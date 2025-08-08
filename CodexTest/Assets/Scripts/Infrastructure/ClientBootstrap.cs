using Game.Domain.ECS;
using Game.Networking;
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

        private void Start()
        {
            networkManager = new NetworkManager();
            networkManager.StartClient(address, port);

            var playerEntity = new Entity(0); // Match server-spawned entity ID
            inputSender.Initialize(networkManager, playerEntity);
            snapshotReceiver.Initialize(networkManager, playerVisual);
            if (cameraFollow != null && playerVisual != null)
            {
                cameraFollow.SetTarget(playerVisual);
            }
        }

        private void Update()
        {
            networkManager.Update();
        }

        private void OnDestroy()
        {
            networkManager?.Dispose();
        }
    }
}
