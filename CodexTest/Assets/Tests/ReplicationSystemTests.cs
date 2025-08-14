using System.Collections.Generic;
using Game.Domain.ECS;
using Game.Domain.Events;
using GameEventBus = Game.EventBus.EventBus;
using Game.Networking;
using Game.Networking.Messages;
using Game.Systems;
using NUnit.Framework;
using UnityEngine;

namespace Tests
{
    public class ReplicationSystemTests
    {
        private class FakeTransport : INetworkTransport
        {
            public readonly List<NetworkMessage> Sent = new();
            public bool IsServer => false;
            public bool IsConnected => true;
            public event System.Action<int> OnClientConnected;
            public event System.Action<int> OnClientDisconnected;
            public event System.Action<int, byte[]> OnData;
            public void StartClient(string address, ushort port) { }
            public void StartServer(ushort port) { }
            public void SendBytes(byte[] bytes) { }
            public void SendBytes(int connectionId, byte[] bytes) { }
            public void SendMessage<T>(T message)
            {
                if (message is NetworkMessage nm)
                {
                    Sent.Add(nm);
                }
            }
            public void SendMessage<T>(int connectionId, T message)
            {
                if (message is NetworkMessage nm)
                {
                    Sent.Add(nm);
                }
            }
            public void Update() { }
            public void Dispose() { }
        }

        [Test]
        public void PositionChangedEvent_SendsSnapshotMessage()
        {
            var network = new FakeTransport();
            var eventBus = new GameEventBus();
            var system = new ReplicationSystem(network, eventBus);

            var entity = new Entity(42);
            var position = new Vector3(1f, 2f, 3f);
            eventBus.Publish(new PositionChangedEvent(entity, position));

            Assert.AreEqual(1, network.Sent.Count);
            var message = network.Sent[0];
            Assert.AreEqual(MessageType.PositionSnapshot, message.Type);

            var snapshot = JsonUtility.FromJson<PositionSnapshot>(message.Payload);
            Assert.AreEqual(entity.Id, snapshot.EntityId);
            Assert.AreEqual(position, snapshot.Position);
        }
    }
}

