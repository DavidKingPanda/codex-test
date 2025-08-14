using System.Collections.Generic;
using Game.Domain.ECS;
using Game.Domain.Events;
using Game.EventBus;
using Game.Networking;
using Game.Networking.Messages;
using Game.Systems;
using NUnit.Framework;
using UnityEngine;

namespace Tests
{
    public class ReplicationSystemTests
    {
        private class TestNetworkManager : NetworkManager
        {
            public readonly List<NetworkMessage> Sent = new();
            public override void SendMessage<T>(T message)
            {
                if (message is NetworkMessage nm)
                {
                    Sent.Add(nm);
                }
            }
        }

        [Test]
        public void PositionChangedEvent_SendsSnapshotMessage()
        {
            var network = new TestNetworkManager();
            var eventBus = new EventBus();
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

