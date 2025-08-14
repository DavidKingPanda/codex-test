using System;
using System.Collections.Generic;

namespace Game.Infrastructure
{
    /// <summary>Simple event bus for decoupled communication between systems.</summary>
    public class EventBus
    {
        private readonly Dictionary<Type, List<Delegate>> _subscribers = new();

        public void Subscribe<T>(Action<T> handler)
        {
            var type = typeof(T);
            if (!_subscribers.TryGetValue(type, out var list))
            {
                list = new List<Delegate>();
                _subscribers[type] = list;
            }
            list.Add(handler);
        }

        public void Publish<T>(T evt)
        {
            var type = typeof(T);
            if (_subscribers.TryGetValue(type, out var list))
            {
                foreach (var handler in list)
                {
                    ((Action<T>)handler)?.Invoke(evt);
                }
            }
        }
    }
}
