using System;
using System.Collections.Generic;
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
        private readonly Dictionary<int, int> _connectionIdMap = new();
        private int _nextClientId = 1;
        public event Action<int, DataStreamReader> OnData;
        public event Action<int> OnClientConnected;
        public event Action<int> OnClientDisconnected;

        public void StartClient(string address, ushort port)
        {
            _driver = NetworkDriver.Create();
            _connections = new NativeList<NetworkConnection>(1, Allocator.Persistent);
            var endpoint = NetworkEndpoint.Parse(address, port);
            var connection = _driver.Connect(endpoint);
            _connections.Add(connection);
            _connectionIdMap[connection.InternalId] = 0;
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
        }

        public void Update()
        {
            if (!_driver.IsCreated)
                return;
            _driver.ScheduleUpdate().Complete();

            // Accept incoming connections on the server.
            NetworkConnection connection;
            while ((connection = _driver.Accept()).IsCreated)
            {
                var clientId = _nextClientId++;
                _connections.Add(connection);
                _connectionIdMap[connection.InternalId] = clientId;
                OnClientConnected?.Invoke(clientId);
            }

            for (int i = 0; i < _connections.Length; i++)
            {
                if (!_connections[i].IsCreated)
                    continue;
                DataStreamReader stream;
                NetworkEvent.Type cmd;
                while ((cmd = _driver.PopEventForConnection(_connections[i], out stream)) != NetworkEvent.Type.Empty)
                {
                    _connectionIdMap.TryGetValue(_connections[i].InternalId, out var clientId);
                    if (cmd == NetworkEvent.Type.Data)
                    {
                        OnData?.Invoke(clientId, stream);
                    }
                    else if (cmd == NetworkEvent.Type.Disconnect)
                    {
                        OnClientDisconnected?.Invoke(clientId);
                        _connectionIdMap.Remove(_connections[i].InternalId);
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

        public void SendBytes(byte[] bytes)
        {
            if (_connections.Length == 0)
                return;
            using var nativeArray = new NativeArray<byte>(bytes, Allocator.Temp);
            for (int i = 0; i < _connections.Length; i++)
            {
                if (!_connections[i].IsCreated)
                    continue;
                if (_driver.BeginSend(_connections[i], out var writer) == 0)
                {
                    writer.WriteBytes(nativeArray);
                    _driver.EndSend(writer);
                }
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
            _connectionIdMap.Clear();
        }
    }
}
