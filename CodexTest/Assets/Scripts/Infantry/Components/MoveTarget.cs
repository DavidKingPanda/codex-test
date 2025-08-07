using Unity.Entities;
using Unity.Mathematics;

namespace RTS.Infantry
{
    /// <summary>Desired destination for a unit.</summary>
    public struct MoveTarget : IComponentData
    {
        public float3 Position;
    }
}
