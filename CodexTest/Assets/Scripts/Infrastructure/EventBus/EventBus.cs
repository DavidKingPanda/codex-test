using System;
using System.Collections.Generic;

namespace Game.Infrastructure
{
    /// <summary>
    /// Simple event bus for decoupled communication between systems.
    /// </summary>
    public class EventBus
    {
        private readonly Dictionary<Type, List<Delegate>> _subscribers = new();

        public void Subscribe<T>(Action<T> handler)
        {
            var type = typeof(T);
            if (!_subscribers.TryGetValue(type, out var handlers))
            {
                handlers = new List<Delegate>();
                _subscribers[type] = handlers;
            }
            handlers.Add(handler);
        }

        public void Unsubscribe<T>(Action<T> handler)
        {
            var type = typeof(T);
            if (_subscribers.TryGetValue(type, out var handlers))
            {
                handlers.Remove(handler);
            }
        }

        public void Publish<T>(T evt)
        {
            var type = typeof(T);
            if (_subscribers.TryGetValue(type, out var handlers))
            {
                foreach (var handler in handlers)
                {
                    ((Action<T>)handler)?.Invoke(evt);
                }
            }
        }
    }
}
