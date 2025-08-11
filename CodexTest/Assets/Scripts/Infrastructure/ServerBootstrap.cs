using Game.Components;
using Game.Domain;
using Game.Domain.ECS;
using Game.Infrastructure;
using Game.Networking;
using Game.Systems;
using Game.Domain.Events;
using UnityEngine;

namespace Game.Infrastructure
{
    /// <summary>
    /// Initializes server-side systems and runs the authoritative simulation.
    /// </summary>
    public class ServerBootstrap : MonoBehaviour
    {
        [SerializeField] private ushort port = 7777;

        private World world;
        private EventBus eventBus;
        private NetworkManager networkManager;
        private MovementSystem movementSystem;
        private ReplicationSystem replicationSystem;
        private JumpSystem jumpSystem;
        private ServerCommandDispatcher dispatcher;
        private CommandHandler commandHandler;

        private void Start()
        {
            eventBus = new EventBus();
            world = new World();
            networkManager = new NetworkManager();
            networkManager.StartServer(port);
            networkManager.OnClientConnected += OnClientConnected;
            networkManager.OnClientDisconnected += OnClientDisconnected;
            dispatcher = new ServerCommandDispatcher(networkManager, eventBus);
            movementSystem = new MovementSystem(world, eventBus);
            replicationSystem = new ReplicationSystem(networkManager, eventBus);
            jumpSystem = new JumpSystem(world, eventBus);
            commandHandler = new CommandHandler(eventBus);

            // Spawn a single player entity at the origin.
            var player = world.CreateEntity();
            world.AddComponent(player, new PositionComponent { Value = Vector3.zero });
            eventBus.Publish(new PositionChangedEvent(player, Vector3.zero));
        }

        private void Update()
        {
            networkManager.Update();
            movementSystem.Update(world, Time.deltaTime);
            replicationSystem.Update(world, Time.deltaTime);
            jumpSystem.Update(world, Time.deltaTime);
        }

        private void OnDestroy()
        {
            if (networkManager != null)
            {
                networkManager.OnClientConnected -= OnClientConnected;
                networkManager.OnClientDisconnected -= OnClientDisconnected;
                networkManager.Dispose();
            }
        }

        private void OnClientConnected(int clientId)
        {
            Debug.Log($"Client {clientId} connected");
        }

        private void OnClientDisconnected(int clientId)
        {
            Debug.Log($"Client {clientId} disconnected");
        }
    }
}
