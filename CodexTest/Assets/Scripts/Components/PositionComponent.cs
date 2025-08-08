using UnityEngine;
using Game.Domain.ECS;

namespace Game.Components
{
    /// <summary>
    /// Holds world position of an entity.
    /// Pure data only.
    /// </summary>
    public struct PositionComponent : IComponent
    {
        public Vector3 Value;
    }
}
