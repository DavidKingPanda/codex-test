using Unity.Entities;
using UnityEngine;

namespace RTS.Infantry
{
    /// <summary>
    /// Helper MonoBehaviour holding an Entity created at conversion time.
    /// </summary>
    public class EntityReference : MonoBehaviour
    {
        public Entity Entity;
    }
}
