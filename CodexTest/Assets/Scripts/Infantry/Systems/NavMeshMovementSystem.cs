using Unity.Entities;
using UnityEngine;

namespace RTS.Infantry
{
    /// <summary>
    /// Drives NavMeshAgents toward their MoveTarget positions.
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class NavMeshMovementSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities
                .WithStructuralChanges()
                .ForEach((Entity entity, NavAgent agent, in MoveTarget target) =>
                {
                    if (agent.Agent == null)
                        return;

                    agent.Agent.SetDestination(target.Position);

                    if (!agent.Agent.pathPending && agent.Agent.remainingDistance <= agent.Agent.stoppingDistance)
                    {
                        EntityManager.RemoveComponent<MoveTarget>(entity);
                    }
                }).Run();
        }
    }
}
