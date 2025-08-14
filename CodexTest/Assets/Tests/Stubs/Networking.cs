using System;
using Game.Domain.ECS;
using UnityEngine;

namespace Game.Networking
{
    /// <summary>Stub network manager used for tests.</summary>
    public class NetworkManager
    {
        public virtual void SendMessage<T>(T message) { }
    }
}

namespace Game.Networking.Messages
{
    public enum MessageType
    {
        MoveCommand = 1,
        PositionSnapshot = 2,
        JumpCommand = 3,
        SpawnPlayer = 4,
        Ping = 5,
        HungerSnapshot = 6,
        StaminaSnapshot = 7
    }

    /// <summary>Envelope for network messages.</summary>
    [Serializable]
    public struct NetworkMessage
    {
        public MessageType Type;
        public string Payload;
        public NetworkMessage(MessageType type, string payload)
        {
            Type = type;
            Payload = payload;
        }
    }

    /// <summary>Snapshot message containing entity position.</summary>
    [Serializable]
    public struct PositionSnapshot
    {
        public int EntityId;
        public Vector3 Position;
        public PositionSnapshot(Entity entity, Vector3 position)
        {
            EntityId = entity.Id;
            Position = position;
        }
    }
}
