using System;
using Game.Networking;
using Game.Networking.Messages;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;

namespace Game.Infrastructure
{
    /// <summary>
    /// Periodically sends ping messages and logs round-trip latency.
    /// </summary>
    public class NetworkLatencyLogger : MonoBehaviour
    {
        [SerializeField] private float pingInterval = 1f;
        private NetworkManager _networkManager;
        private float _timer;

        /// <summary>
        /// Injects the network manager and subscribes to data events.
        /// </summary>
        public void Initialize(NetworkManager manager)
        {
            _networkManager = manager;
            _networkManager.OnData += OnDataReceived;
        }

        private void Update()
        {
            if (_networkManager == null || !_networkManager.IsConnected)
                return;

            _timer += Time.deltaTime;
            if (_timer >= pingInterval)
            {
                _timer = 0f;
                var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                var ping = new Ping(timestamp);
                var payload = JsonUtility.ToJson(ping);
                var message = new NetworkMessage(MessageType.Ping, payload);
                _networkManager.SendMessage(message);
            }
        }

        private void OnDataReceived(NetworkConnection connection, DataStreamReader stream)
        {
            using var bytes = new NativeArray<byte>(stream.Length, Allocator.Temp);
            stream.ReadBytes(bytes);
            var json = System.Text.Encoding.UTF8.GetString(bytes.ToArray());
            var message = JsonUtility.FromJson<NetworkMessage>(json);
            if (message.Type != MessageType.Ping)
                return;

            var ping = JsonUtility.FromJson<Ping>(message.Payload);
            var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var rtt = now - ping.Timestamp;
            Debug.Log($"RTT: {rtt} ms");
        }

        private void OnDestroy()
        {
            if (_networkManager != null)
            {
                _networkManager.OnData -= OnDataReceived;
            }
        }
    }
}
