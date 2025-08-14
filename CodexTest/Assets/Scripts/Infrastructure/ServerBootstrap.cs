using Game.Components;
using Game.Domain;
using Game.Domain.ECS;
using Game.Domain.Events;
using Game.Networking;
using Game.Networking.Transport;
using Game.Networking.Messages;
using Game.Systems;
using Game.Utils;
using System.Collections.Generic;
using GameEventBus = Game.EventBus.EventBus;
using UnityEngine;

namespace Game.Infrastructure
{
    /// <summary>
    /// Initializes server-side systems and runs the authoritative simulation.
    /// </summary>
    public class ServerBootstrap : MonoBehaviour
    {
        [SerializeField] private ushort port = 80;
        [SerializeField] private SurvivalConfig survivalConfig;
        [SerializeField] private MovementConfig movementConfig;

        private World world;
        private GameEventBus eventBus;
        private INetworkTransport networkManager;
        private MovementSystem movementSystem;
        private ReplicationSystem replicationSystem;
        private JumpSystem jumpSystem;
        private SurvivalSystem survivalSystem;
        private SurvivalReplicationSystem survivalReplicationSystem;
        private ServerCommandDispatcher dispatcher;
        private CommandHandler commandHandler;
        private Dictionary<int, Entity> connectionToEntity;
        private float tickAccumulator;
        private float fixedDeltaTime;

        private bool _initialized;

        private void Start()
        {
            eventBus = new GameEventBus();
            world = new World();
            networkManager = new UnityTransportAdapter();
            networkManager.StartServer(port);
            if (survivalConfig == null)
            {
                survivalConfig = ScriptableObject.CreateInstance<SurvivalConfig>();
            }
            if (movementConfig == null)
            {
                movementConfig = ScriptableObject.CreateInstance<MovementConfig>();
            }
            connectionToEntity = new Dictionary<int, Entity>();
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

        private void OnClientConnected(int connectionId)
        {
            var entity = world.CreateEntity();
            world.AddComponent(entity, new PositionComponent { Value = Vector3.zero });
            var speedComponent = new MovementSpeedComponent
            {
                WalkSpeed = movementConfig.WalkSpeed,
                RunSpeed = movementConfig.RunSpeed
            };
            world.AddComponent(entity, speedComponent);
            world.AddComponent(entity, new JumpSettingsComponent
            {
                JumpForce = movementConfig.JumpForce,
                Gravity = movementConfig.Gravity
            });
            world.AddComponent(entity, new StaminaComponent
            {
                Current = survivalConfig.MaxStamina,
                Max = survivalConfig.MaxStamina,
                DrainPerSecond = survivalConfig.StaminaDrainPerSecond,
                RegenPerSecond = survivalConfig.StaminaRegenPerSecond
            });
            world.AddComponent(entity, new HungerComponent
            {
                Current = survivalConfig.MaxHunger,
                Max = survivalConfig.MaxHunger,
                DrainPerSecond = survivalConfig.HungerDrainPerSecond
            });
            world.AddComponent(entity, new MovementStateComponent { IsRunning = false });
            eventBus.Publish(new PositionChangedEvent(entity, Vector3.zero));
            eventBus.Publish(new StaminaChangedEvent(entity, survivalConfig.MaxStamina, survivalConfig.MaxStamina));
            eventBus.Publish(new HungerChangedEvent(entity, survivalConfig.MaxHunger, survivalConfig.MaxHunger));
            connectionToEntity[connectionId] = entity;

            var spawn = new SpawnPlayer(entity, speedComponent.WalkSpeed, speedComponent.RunSpeed, movementConfig.JumpForce, movementConfig.Gravity);
            var payload = JsonUtility.ToJson(spawn);
            var message = new NetworkMessage(MessageType.SpawnPlayer, payload);
            networkManager.SendMessage(connectionId, message);
        }

        private void OnClientDisconnected(int connectionId)
        {
            connectionToEntity.Remove(connectionId);
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

