using System;
using Game.Domain.ECS;

namespace Game.Components
{
    /// <summary>
    /// Defines jump force and gravity for an entity.
    /// </summary>
    [Serializable]
    public struct JumpSettingsComponent : IComponent
    {
        public float JumpForce;
        public float Gravity;
    }
}
