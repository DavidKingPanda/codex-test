using Game.Components;
using Game.Domain;
using Game.Domain.ECS;
using Game.Domain.Events;
using Game.Networking;
using Game.Networking.Messages;
using Game.Systems;
using Game.Utils;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

namespace Game.Infrastructure
{
    /// <summary>
    /// Initializes server-side systems and runs the authoritative simulation.
    /// </summary>
    public class ServerBootstrap : MonoBehaviour
    {
        [SerializeField] private ushort port = 80;

        private World world;
        private EventBus eventBus;
        private NetworkManager networkManager;
        private MovementSystem movementSystem;
        private ReplicationSystem replicationSystem;
        private JumpSystem jumpSystem;
        private SurvivalSystem survivalSystem;
        private SurvivalReplicationSystem survivalReplicationSystem;
        private ServerCommandDispatcher dispatcher;
        private CommandHandler commandHandler;
        private Dictionary<NetworkConnection, Entity> connectionToEntity;
        private float tickAccumulator;
        private float fixedDeltaTime;

        private bool _initialized;

        private void Start()
        {
            eventBus = new EventBus();
            world = new World();
            networkManager = new NetworkManager();
            networkManager.StartServer(port);
            connectionToEntity = new Dictionary<NetworkConnection, Entity>();
            networkManager.OnClientConnected += OnClientConnected;
            networkManager.OnClientDisconnected += OnClientDisconnected;
            dispatcher = new ServerCommandDispatcher(networkManager, eventBus, connectionToEntity);
            movementSystem = new MovementSystem(world, eventBus);
            replicationSystem = new ReplicationSystem(networkManager, eventBus);
            jumpSystem = new JumpSystem(world, eventBus);
            survivalSystem = new SurvivalSystem(world, eventBus);
            survivalReplicationSystem = new SurvivalReplicationSystem(networkManager, eventBus);
            commandHandler = new CommandHandler(eventBus);
            fixedDeltaTime = 1f / Constants.ServerTickRate;
            _initialized = true;
        }

        private void OnClientConnected(NetworkConnection connection)
        {
            var entity = world.CreateEntity();
            world.AddComponent(entity, new PositionComponent { Value = Vector3.zero });
            world.AddComponent(entity, new MovementSpeedComponent { WalkSpeed = 2f, RunSpeed = 4f });
            world.AddComponent(entity, new StaminaComponent { Current = 100f, Max = 100f, DrainPerSecond = 10f, RegenPerSecond = 5f });
            world.AddComponent(entity, new HungerComponent { Current = 100f, Max = 100f, DrainPerSecond = 1f });
            world.AddComponent(entity, new MovementStateComponent { IsRunning = false });
            eventBus.Publish(new PositionChangedEvent(entity, Vector3.zero));
            eventBus.Publish(new StaminaChangedEvent(entity, 100f, 100f));
            eventBus.Publish(new HungerChangedEvent(entity, 100f, 100f));
            connectionToEntity[connection] = entity;

            var spawn = new SpawnPlayer(entity);
            var payload = JsonUtility.ToJson(spawn);
            var message = new NetworkMessage(MessageType.SpawnPlayer, payload);
            networkManager.SendMessage(connection, message);
        }

        private void OnClientDisconnected(NetworkConnection connection)
        {
            connectionToEntity.Remove(connection);
        }

        private void Update()
        {
            if (!_initialized || networkManager == null)
            {
                return;
            }

            networkManager.Update();
            tickAccumulator += Time.deltaTime;
            while (tickAccumulator >= fixedDeltaTime)
            {
                movementSystem.Update(world, fixedDeltaTime);
                replicationSystem.Update(world, fixedDeltaTime);
                jumpSystem.Update(world, fixedDeltaTime);
                survivalSystem.Update(world, fixedDeltaTime);
                survivalReplicationSystem.Update(world, fixedDeltaTime);
                tickAccumulator -= fixedDeltaTime;
            }
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
    }
}

