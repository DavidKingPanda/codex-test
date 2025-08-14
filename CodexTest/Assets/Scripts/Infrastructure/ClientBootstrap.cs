using Game.Domain.ECS;
using Game.Networking;
using Game.Networking.Messages;
using System.Text;
using Game.Presentation;
using GameEventBus = Game.EventBus.EventBus;
using UnityEngine;

namespace Game.Infrastructure
{
    /// <summary>
    /// Starts the client network connection and wires up input and camera.
    /// </summary>
    public class ClientBootstrap : MonoBehaviour
    {
        [SerializeField] private string address = "127.0.0.1";
        [SerializeField] private ushort port = 80;
        [SerializeField] private ClientInputSender inputSender;
        [SerializeField] private Transform playerVisual;
        [SerializeField] private CameraController cameraController;
        [SerializeField] private ClientSnapshotReceiver snapshotReceiver;
        [SerializeField] private StatsSnapshotReceiver statsReceiver;
        [SerializeField] private SurvivalUI survivalUI;
        [SerializeField] private NetworkLatencyLogger latencyLogger;
        [SerializeField] private PingDisplay pingDisplay;

        [SerializeField] private MonoBehaviour transport;
        private INetworkTransport networkManager;
        private GameEventBus eventBus;
        private bool playerInitialized;

        private void Awake()
        {
            // Allow overriding address/port via command line or environment variables.
            var args = System.Environment.GetCommandLineArgs();
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "-server" && i + 1 < args.Length)
                {
                    address = args[i + 1];
                }
                else if (args[i] == "-port" && i + 1 < args.Length && ushort.TryParse(args[i + 1], out var parsedPort))
                {
                    port = parsedPort;
                }
            }

            var envAddress = System.Environment.GetEnvironmentVariable("SERVER_ADDRESS");
            if (!string.IsNullOrWhiteSpace(envAddress))
            {
                address = envAddress;
            }

            var envPort = System.Environment.GetEnvironmentVariable("SERVER_PORT");
            if (!string.IsNullOrWhiteSpace(envPort) && ushort.TryParse(envPort, out var envParsedPort))
            {
                port = envParsedPort;
            }
        }

        private void Start()
        {
            eventBus = new GameEventBus();
            networkManager = transport as INetworkTransport;
            if (networkManager == null)
            {
                Debug.LogError("Transport does not implement INetworkTransport");
                enabled = false;
                return;
            }
            networkManager.StartClient(address, port);
            networkManager.OnData += OnDataReceived;
            if (latencyLogger != null)
            {
                latencyLogger.Initialize(networkManager);
                if (pingDisplay != null)
                {
                    latencyLogger.OnPingUpdated += pingDisplay.UpdatePing;
                }
            }
        }

        private void OnDataReceived(int connectionId, byte[] data)
        {
            var json = Encoding.UTF8.GetString(data);
            var message = JsonUtility.FromJson<NetworkMessage>(json);
            if (message.Type != MessageType.SpawnPlayer || playerInitialized)
                return;

            var spawn = JsonUtility.FromJson<SpawnPlayer>(message.Payload);
            var entity = new Entity(spawn.EntityId);
            if (inputSender != null)
            {
                inputSender.Initialize(networkManager, eventBus, entity, playerVisual, spawn.WalkSpeed, spawn.RunSpeed, spawn.JumpForce, spawn.Gravity);
            }
            snapshotReceiver.Initialize(networkManager, playerVisual);
            snapshotReceiver.RegisterEntity(entity.Id, playerVisual);
            if (statsReceiver != null)
            {
                statsReceiver.Initialize(networkManager, eventBus, entity);
            }
            if (survivalUI != null)
            {
                survivalUI.Initialize(eventBus);
            }
            if (cameraController != null && playerVisual != null)
            {
                cameraController.SetTarget(playerVisual);
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
            if (latencyLogger != null && pingDisplay != null)
            {
                latencyLogger.OnPingUpdated -= pingDisplay.UpdatePing;
            }
        }
    }
}
