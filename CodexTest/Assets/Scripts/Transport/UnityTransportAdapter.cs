using System;
using System.Collections.Generic;
using System.Text;
using Game.Networking;
using Unity.Collections;
using Unity.Networking.Transport;
using Unity.Networking.Transport.Utilities;
using UnityEngine;

namespace Game.Networking.Transport
{
    /// <summary>
    /// Adapter that wraps Unity.Transport and exposes it via the INetworkTransport interface.
    /// This file resides outside of any asmdef so it can directly reference Unity Transport types.
    /// </summary>
    public class UnityTransportAdapter : INetworkTransport
    {
        private NetworkDriver _driver;
        private NativeList<NetworkConnection> _connections;
        private List<int> _connectionIds;
        private int _nextConnectionId;

        public bool IsServer { get; private set; }

        public event Action<int> OnClientConnected;
        public event Action<int> OnClientDisconnected;
        public event Action<int, byte[]> OnData;

        public bool IsConnected =>
            _connections.IsCreated && _connections.Length > 0 && _connections[0].IsCreated;

        public void StartClient(string address, ushort port)
        {
            var settings = new NetworkSettings();
            _driver = NetworkDriver.Create(settings);

            _connections = new NativeList<NetworkConnection>(1, Allocator.Persistent);
            _connectionIds = new List<int>(1);

            var endpoint = NetworkEndpoint.Parse(address, port, NetworkFamily.Ipv4);
            var connection = _driver.Connect(endpoint);
            _connections.Add(connection);
            _connectionIds.Add(_nextConnectionId++);
            IsServer = false;
        }

        public void StartServer(ushort port)
        {
            var settings = new NetworkSettings();
            _driver = NetworkDriver.Create(settings);

            _connections = new NativeList<NetworkConnection>(16, Allocator.Persistent);
            _connectionIds = new List<int>(16);

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
                    _connectionIds.Add(_nextConnectionId);
                    OnClientConnected?.Invoke(_nextConnectionId);
                    _nextConnectionId++;
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
                    int connectionId = _connectionIds[i];
                    if (cmd == NetworkEvent.Type.Data)
                    {
                        using var bytes = new NativeArray<byte>(stream.Length, Allocator.Temp);
                        stream.ReadBytes(bytes);
                        OnData?.Invoke(connectionId, bytes.ToArray());
                    }
                    else if (cmd == NetworkEvent.Type.Disconnect)
                    {
                        OnClientDisconnected?.Invoke(connectionId);
                        _connections[i] = default;
                    }
                }

                if (!_connections[i].IsCreated)
                {
                    _connections.RemoveAtSwapBack(i);
                    int lastIndex = _connectionIds.Count - 1;
                    _connectionIds[i] = _connectionIds[lastIndex];
                    _connectionIds.RemoveAt(lastIndex);
                    i--;
                }
            }
        }

        private void SendBytesInternal(NetworkConnection connection, byte[] bytes)
        {
            if (!connection.IsCreated)
                return;

            using var nativeArray = new NativeArray<byte>(bytes, Allocator.Temp);
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
                    SendBytesInternal(_connections[i], bytes);
                }
            }
            else
            {
                SendBytesInternal(_connections[0], bytes);
            }
        }

        public void SendBytes(int connectionId, byte[] bytes)
        {
            if (!_connections.IsCreated)
                return;

            for (int i = 0; i < _connectionIds.Count; i++)
            {
                if (_connectionIds[i] == connectionId)
                {
                    SendBytesInternal(_connections[i], bytes);
                    break;
                }
            }
        }

        public void SendMessage<T>(T message)
        {
            var json = JsonUtility.ToJson(message);
            SendBytes(Encoding.UTF8.GetBytes(json));
        }

        public void SendMessage<T>(int connectionId, T message)
        {
            var json = JsonUtility.ToJson(message);
            SendBytes(connectionId, Encoding.UTF8.GetBytes(json));
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

