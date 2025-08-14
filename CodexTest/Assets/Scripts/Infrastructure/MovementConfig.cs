using UnityEngine;

namespace Game.Infrastructure
{
    /// <summary>
    /// ScriptableObject holding movement speed settings.
    /// </summary>
    [CreateAssetMenu(menuName = "Config/MovementConfig")]
    public class MovementConfig : ScriptableObject
    {
        public float WalkSpeed = 2f;
        public float RunSpeed = 4f;
        /// <summary>
        /// Upward velocity applied when jumping.
        /// </summary>
        public float JumpForce = 5f;
        /// <summary>
        /// Gravity applied to vertical motion.
        /// </summary>
        public float Gravity = -9.81f;
    }
}
