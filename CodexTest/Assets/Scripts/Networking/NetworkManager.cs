using System;
using System.Text;
using Unity.Collections;
using Unity.Networking.Transport;
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

        public void StartClient(string address, ushort port)
        {
            _driver = NetworkDriver.Create();
            _connections = new NativeList<NetworkConnection>(1, Allocator.Persistent);
            var endpoint = NetworkEndpoint.Parse(address, port);
            var connection = _driver.Connect(endpoint);
            _connections.Add(connection);
            IsServer = false;
        }

        public void StartServer(ushort port)
        {
            _driver = NetworkDriver.Create();
            _connections = new NativeList<NetworkConnection>(16, Allocator.Persistent);
            var endpoint = NetworkEndpoint.AnyIpv4;
            endpoint.Port = port;
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
            if (_driver.BeginSend(connection, out var writer) == 0)
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

        public void SendMessage<T>(T message)
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
