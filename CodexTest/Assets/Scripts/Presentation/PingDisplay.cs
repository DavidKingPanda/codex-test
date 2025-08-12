using Game.Infrastructure;
using TMPro;
using UnityEngine;

namespace Game.Presentation
{
    /// <summary>
    /// Displays the latest network ping value on a UI text element.
    /// Presentation only; no gameplay logic.
    /// </summary>
    public class PingDisplay : MonoBehaviour
    {
        [SerializeField] private TMP_Text pingText;
        [SerializeField] private NetworkLatencyLogger latencyLogger;

        private void OnEnable()
        {
            if (latencyLogger != null)
            {
                latencyLogger.OnPingUpdated += HandlePingUpdated;
            }
        }

        private void OnDisable()
        {
            if (latencyLogger != null)
            {
                latencyLogger.OnPingUpdated -= HandlePingUpdated;
            }
        }

        private void HandlePingUpdated(long rtt)
        {
            if (pingText != null)
            {
                pingText.text = $"Ping: {rtt} ms";
            }
        }
    }
}

