using System;
using System.Collections.Generic;

namespace Game.Domain.ECS
{
    /// <summary>
    /// Marker interface for components.
    /// </summary>
    public interface IComponent { }

    /// <summary>
    /// Identifier for entities in the world.
    /// </summary>
    public readonly struct Entity
    {
        public readonly int Id;
        public Entity(int id) => Id = id;
    }

    /// <summary>
    /// Interface for all ECS systems.
    /// </summary>
    public interface ISystem
    {
        void Update(World world, float deltaTime);
    }

    /// <summary>
    /// Minimal ECS world used for unit tests.
    /// Stores components in dictionaries keyed by entity id.
    /// </summary>
    public class World
    {
        private int _nextId;
        private readonly Dictionary<Type, Dictionary<int, IComponent>> _components = new();

        public Entity CreateEntity() => new Entity(_nextId++);

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
            if (_components.TryGetValue(type, out var map) && map.TryGetValue(entity.Id, out var value))
            {
                component = (T)value;
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
    }
}
