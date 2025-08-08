using System.Text;
using System.Text.Json;
using Unity.Collections;
using Unity.Networking.Transport;

using Game.Domain.Commands;
using Game.Networking;

namespace Game.Infrastructure
{
    /// <summary>
    /// Listens for incoming network data and publishes commands to the event bus.
    /// </summary>
    public class ServerCommandDispatcher : IDisposable
    {
        private readonly NetworkManager _networkManager;
        private readonly EventBus _eventBus;

        public ServerCommandDispatcher(NetworkManager networkManager, EventBus eventBus)
        {
            _networkManager = networkManager;
            _eventBus = eventBus;
            _networkManager.OnData += OnDataReceived;
        }

        private void OnDataReceived(DataStreamReader stream)
        {
            using var bytes = new NativeArray<byte>(stream.Length, Allocator.Temp);
            stream.ReadBytes(bytes);
            var json = Encoding.UTF8.GetString(bytes.ToArray());
            var move = JsonSerializer.Deserialize<MoveCommand>(json);
            if (move != null)
            {
                _eventBus.Publish(move);
            }
        }

        public void Dispose()
        {
            _networkManager.OnData -= OnDataReceived;
        }
    }
}
