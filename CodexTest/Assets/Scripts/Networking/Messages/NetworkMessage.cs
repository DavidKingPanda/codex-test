using System;

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

    /// <summary>
    /// Envelope for all network messages.
    /// </summary>
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
}
