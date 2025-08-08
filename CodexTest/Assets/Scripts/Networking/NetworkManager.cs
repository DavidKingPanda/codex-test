using System;
using System.Text;
using System.Text.Json;
using Unity.Collections;
using Unity.Networking.Transport;


namespace Game.Networking
{
    /// <summary>
    /// Minimal wrapper around Unity Transport for sending and receiving raw messages.
    /// Contains no game logic.
    /// </summary>
    public class NetworkManager : IDisposable
    {
        private NetworkDriver _driver;
        private NetworkConnection _connection;
        public event Action<DataStreamReader> OnData;

        public void StartClient(string address, ushort port)
        {
            _driver = NetworkDriver.Create();
            var endpoint = NetworkEndPoint.Parse(address, port);
            _connection = _driver.Connect(endpoint);
        }

        public void StartServer(ushort port)
        {
            _driver = NetworkDriver.Create();
            var endpoint = NetworkEndPoint.AnyIpv4;
            endpoint.Port = port;
            if (_driver.Bind(endpoint) != 0)
            {
                throw new Exception("Failed to bind to port" + port);
            }
            _driver.Listen();
        }

        public void Update()
        {
            _driver.ScheduleUpdate().Complete();

            if (_driver.IsCreated && !_connection.IsCreated)
            {
                // Accept incoming connection on the server.
                var connection = _driver.Accept();
                if (connection.IsCreated)
                {
                    _connection = connection;
                }
            }

            if (_connection.IsCreated)
            {
                DataStreamReader stream;
                NetworkEvent.Type cmd;
                while ((cmd = _driver.PopEventForConnection(_connection, out stream)) != NetworkEvent.Type.Empty)
                {
                    if (cmd == NetworkEvent.Type.Data)
                    {
                        OnData?.Invoke(stream);
                    }
                    else if (cmd == NetworkEvent.Type.Disconnect)
                    {
                        _connection = default;
                    }
                }
            }
        }

        public void SendBytes(byte[] bytes)
        {
            if (!_connection.IsCreated)
                return;
            using var nativeArray = new NativeArray<byte>(bytes, Allocator.Temp);
            var writer = _driver.BeginSend(_connection);
            writer.WriteBytes(nativeArray);
            _driver.EndSend(writer);
        }

        public void SendMessage<T>(T message)
        {
            var json = JsonSerializer.Serialize(message);
            SendBytes(Encoding.UTF8.GetBytes(json));
        }

        public void Dispose()
        {
            if (_driver.IsCreated)
            {
                _driver.Dispose();
            }
        }
    }
}
