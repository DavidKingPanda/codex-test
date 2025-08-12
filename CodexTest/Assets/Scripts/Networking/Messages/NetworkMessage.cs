using System;

namespace Game.Networking.Messages
{
    public enum MessageType
    {
        MoveCommand = 1,
        PositionSnapshot = 2,
        JumpCommand = 3,
        SpawnPlayer = 4
    }

    /// <summary>
    /// Envelope for all network messages.
    /// </summary>
    [Serializable]
    public readonly struct NetworkMessage
    {
        public readonly MessageType Type;
        public readonly string Payload;

        public NetworkMessage(MessageType type, string payload)
        {
            Type = type;
            Payload = payload;
        }
    }
}
