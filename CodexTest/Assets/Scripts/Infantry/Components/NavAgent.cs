using Unity.Entities;
using UnityEngine.AI;

namespace RTS.Infantry
{
    /// <summary>Managed component wrapping a NavMeshAgent reference.</summary>
    public class NavAgent : IComponentData
    {
        public NavMeshAgent Agent;
    }
}
