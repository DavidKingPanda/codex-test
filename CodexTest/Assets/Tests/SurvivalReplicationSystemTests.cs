using System.Collections.Generic;
using Game.Domain.ECS;
using Game.Domain.Events;
using EventBus = Game.EventBus.EventBus;
using Game.Networking;
using Game.Networking.Messages;
using Game.Systems;
using NUnit.Framework;
using UnityEngine;

namespace Tests
{
    public class SurvivalReplicationSystemTests
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
        public void StaminaChangedEvent_SendsSnapshotMessage()
        {
            var network = new TestNetworkManager();
            var eventBus = new EventBus();
            var system = new SurvivalReplicationSystem(network, eventBus);

            var entity = new Entity(10);
            eventBus.Publish(new StaminaChangedEvent(entity, 3f, 5f));

            Assert.AreEqual(1, network.Sent.Count);
            var message = network.Sent[0];
            Assert.AreEqual(MessageType.StaminaSnapshot, message.Type);
            var snapshot = JsonUtility.FromJson<StaminaSnapshot>(message.Payload);
            Assert.AreEqual(entity.Id, snapshot.EntityId);
            Assert.AreEqual(3f, snapshot.Current);
            Assert.AreEqual(5f, snapshot.Max);
        }

        [Test]
        public void HungerChangedEvent_SendsSnapshotMessage()
        {
            var network = new TestNetworkManager();
            var eventBus = new EventBus();
            var system = new SurvivalReplicationSystem(network, eventBus);

            var entity = new Entity(11);
            eventBus.Publish(new HungerChangedEvent(entity, 2f, 5f));

            Assert.AreEqual(1, network.Sent.Count);
            var message = network.Sent[0];
            Assert.AreEqual(MessageType.HungerSnapshot, message.Type);
            var snapshot = JsonUtility.FromJson<HungerSnapshot>(message.Payload);
            Assert.AreEqual(entity.Id, snapshot.EntityId);
            Assert.AreEqual(2f, snapshot.Current);
            Assert.AreEqual(5f, snapshot.Max);
        }
    }
}
