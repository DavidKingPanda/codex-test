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
    }
}
