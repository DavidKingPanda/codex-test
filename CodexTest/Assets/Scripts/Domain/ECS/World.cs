using System;
using System.Collections.Generic;

namespace Game.Domain.ECS
{
    /// <summary>
    /// ECS world containing entities and their components.
    /// Provides basic APIs to manage components.
    /// </summary>
    public class World
    {
        private int _nextEntityId;
        private readonly Dictionary<Type, IDictionary<int, IComponent>> _components = new();

        public Entity CreateEntity()
        {
            return new Entity(_nextEntityId++);
        }

        public void AddComponent<T>(Entity entity, T component) where T : IComponent
        {
            var type = typeof(T);
            if (!_components.TryGetValue(type, out var map))
            {
                map = new Dictionary<int, IComponent>();
                _components[type] = map;
            }
            map[entity.Id] = component;
        }

        public bool TryGetComponent<T>(Entity entity, out T component) where T : IComponent
        {
            component = default;
            var type = typeof(T);
            if (_components.TryGetValue(type, out var map) && map.TryGetValue(entity.Id, out var comp))
            {
                component = (T)comp;
                return true;
            }
            return false;
        }

        public void SetComponent<T>(Entity entity, T component) where T : IComponent
        {
            var type = typeof(T);
            if (_components.TryGetValue(type, out var map))
            {
                map[entity.Id] = component;
            }
        }

        public IEnumerable<(Entity entity, T component)> View<T>() where T : IComponent
        {
            var type = typeof(T);
            if (_components.TryGetValue(type, out var map))
            {
                foreach (var pair in map)
                {
                    yield return (new Entity(pair.Key), (T)pair.Value);
                }
            }
        }
    }
}
