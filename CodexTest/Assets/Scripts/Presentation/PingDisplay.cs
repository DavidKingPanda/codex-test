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

        public void UpdatePing(long rtt)
        {
            if (pingText != null)
            {
                pingText.text = $"Ping: {rtt} ms";
            }
        }
    }
}
