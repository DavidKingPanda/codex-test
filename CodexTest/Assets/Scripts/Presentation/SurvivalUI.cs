using Game.Domain.Events;
using Game.Infrastructure;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Presentation
{
    /// <summary>
    /// Displays hunger and stamina bars for the local player.
    /// </summary>
    public class SurvivalUI : MonoBehaviour
    {
        [SerializeField] private Slider hungerBar;
        [SerializeField] private Slider staminaBar;

        private EventBus _eventBus;

        public void Initialize(EventBus eventBus)
        {
            _eventBus = eventBus;
            _eventBus.Subscribe<HungerChangedEvent>(OnHungerChanged);
            _eventBus.Subscribe<StaminaChangedEvent>(OnStaminaChanged);
        }

        private void OnHungerChanged(HungerChangedEvent evt)
        {
            if (hungerBar != null)
            {
                hungerBar.value = evt.Current / evt.Max;
            }
        }

        private void OnStaminaChanged(StaminaChangedEvent evt)
        {
            if (staminaBar != null)
            {
                staminaBar.value = evt.Current / evt.Max;
            }
        }

        private void OnDestroy()
        {
            if (_eventBus != null)
            {
                _eventBus.Unsubscribe<HungerChangedEvent>(OnHungerChanged);
                _eventBus.Unsubscribe<StaminaChangedEvent>(OnStaminaChanged);
            }
        }
    }
}
