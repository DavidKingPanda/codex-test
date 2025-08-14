using System;
using System.Text;
using Unity.Collections;

using Unity.Networking.Transport.Utilities;
using Unity.Networking.Transport.Error;
using UnityEngine;

namespace Game.Networking
{
    /// <summary>
    /// Minimal wrapper around Unity Transport for sending and receiving raw messages.
    /// Contains no game logic.
    /// </summary>
    public class NetworkManager : IDisposable
    {
        private NetworkDriver _driver;
        private NativeList<NetworkConnection> _connections;
        public bool IsServer { get; private set; }

        public event Action<NetworkConnection> OnClientConnected;
        public event Action<NetworkConnection> OnClientDisconnected;
        public event Action<NetworkConnection, DataStreamReader> OnData;
        /// <summary>
        /// Indicates whether a connection is currently established.
        /// </summary>
        public bool IsConnected =>
            _connections.IsCreated && _connections.Length > 0 && _connections[0].IsCreated;


        public void StartClient(string address, ushort port)
        {
            // Create driver with explicit settings to align with Transport 2.5+ API.
            var settings = new NetworkSettings();
            _driver = NetworkDriver.Create(settings);

            _connections = new NativeList<NetworkConnection>(1, Allocator.Persistent);

            // Explicitly parse IPv4 endpoint.
            var endpoint = NetworkEndpoint.Parse(address, port, NetworkFamily.Ipv4);
            var connection = _driver.Connect(endpoint);
            _connections.Add(connection);
            IsServer = false;
        }

        public void StartServer(ushort port)
        {
            var settings = new NetworkSettings();
            _driver = NetworkDriver.Create(settings);

            _connections = new NativeList<NetworkConnection>(16, Allocator.Persistent);

            // Bind to any IPv4 endpoint, allowing dynamic port selection when port == 0.
            var endpoint = NetworkEndpoint.AnyIpv4.WithPort(port);
            if (_driver.Bind(endpoint) != 0)
            {
                throw new Exception("Failed to bind to port" + port);
            }
            _driver.Listen();
            IsServer = true;
        }

        public void Update()
        {
            if (!_driver.IsCreated)
                return;

            _driver.ScheduleUpdate().Complete();

            if (IsServer)
            {
                NetworkConnection connection;
                while ((connection = _driver.Accept()).IsCreated)
                {
                    _connections.Add(connection);
                    OnClientConnected?.Invoke(connection);
                }
            }

            for (int i = 0; i < _connections.Length; i++)
            {
                if (!_connections[i].IsCreated)
                    continue;

                DataStreamReader stream;
                NetworkEvent.Type cmd;
                while ((cmd = _driver.PopEventForConnection(_connections[i], out stream)) != NetworkEvent.Type.Empty)
                {
                    if (cmd == NetworkEvent.Type.Data)
                    {
                        OnData?.Invoke(_connections[i], stream);
                    }
                    else if (cmd == NetworkEvent.Type.Disconnect)
                    {
                        OnClientDisconnected?.Invoke(_connections[i]);
                        _connections[i] = default;
                    }
                }

                if (!_connections[i].IsCreated)
                {
                    _connections.RemoveAtSwapBack(i);
                    i--;
                }
            }
        }

        private void SendBytes(NetworkConnection connection, byte[] bytes)
        {
            if (!connection.IsCreated)
                return;

            using var nativeArray = new NativeArray<byte>(bytes, Allocator.Temp);

            // New API requires specifying a pipeline when beginning a send operation.
            if (_driver.BeginSend(NetworkPipeline.Null, connection, out var writer) == 0)
            {
                writer.WriteBytes(nativeArray);
                _driver.EndSend(writer);
            }
        }

        public void SendBytes(byte[] bytes)
        {
            if (!_connections.IsCreated || _connections.Length == 0)
                return;

            if (IsServer)
            {
                for (int i = 0; i < _connections.Length; i++)
                {
                    SendBytes(_connections[i], bytes);
                }
            }
            else
            {
                SendBytes(_connections[0], bytes);
            }
        }

        public void SendMessage<T>(NetworkConnection connection, T message)
        {
            var json = JsonUtility.ToJson(message);
            SendBytes(connection, Encoding.UTF8.GetBytes(json));
        }

        public virtual void SendMessage<T>(T message)
        {
            var json = JsonUtility.ToJson(message);
            SendBytes(Encoding.UTF8.GetBytes(json));
        }

        public void Dispose()
        {
            if (_driver.IsCreated)
            {
                _driver.Dispose();
            }
            if (_connections.IsCreated)
            {
                _connections.Dispose();
            }
        }
    }
}
