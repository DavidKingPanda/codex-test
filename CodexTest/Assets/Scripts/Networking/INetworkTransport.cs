using System;

namespace Game.Networking
{
    /// <summary>
    /// Abstraction over the underlying network transport implementation.
    /// Provides a minimal set of operations required by the game logic.
    /// </summary>
    public interface INetworkTransport : IDisposable
    {
        /// <summary>
        /// Fired when a client connects. The integer is a connection identifier.
        /// </summary>
        event Action<int> OnClientConnected;

        /// <summary>
        /// Fired when a client disconnects. The integer is a connection identifier.
        /// </summary>
        event Action<int> OnClientDisconnected;

        /// <summary>
        /// Fired when data is received from a connection.
        /// The first integer is the connection identifier.
        /// </summary>
        event Action<int, byte[]> OnData;

        /// <summary>
        /// Indicates whether this instance is operating as a server.
        /// </summary>
        bool IsServer { get; }

        /// <summary>
        /// Indicates whether a connection is currently established.
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// Starts a client connection to the specified server address and port.
        /// </summary>
        void StartClient(string address, ushort port);

        /// <summary>
        /// Starts listening for client connections on the specified port.
        /// </summary>
        void StartServer(ushort port);

        /// <summary>
        /// Sends raw byte payload to all connected peers (or to the server when operating as a client).
        /// </summary>
        void SendBytes(byte[] bytes);

        /// <summary>
        /// Sends raw byte payload to a specific connection.
        /// </summary>
        void SendBytes(int connectionId, byte[] bytes);

        /// <summary>
        /// Serializes and sends the specified message to all connected peers.
        /// </summary>
        void SendMessage<T>(T message);

        /// <summary>
        /// Serializes and sends the specified message to a specific connection.
        /// </summary>
        void SendMessage<T>(int connectionId, T message);

        /// <summary>
        /// Processes incoming connections and network events.
        /// </summary>
        void Update();
    }
}

