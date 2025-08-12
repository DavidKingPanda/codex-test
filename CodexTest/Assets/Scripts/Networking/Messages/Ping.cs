using System;

namespace Game.Networking.Messages
{
    /// <summary>
    /// Simple message used to measure round-trip time.
    /// </summary>
    [Serializable]
    public struct Ping
    {
        public long Timestamp;

        public Ping(long timestamp)
        {
            Timestamp = timestamp;
        }
    }
}
