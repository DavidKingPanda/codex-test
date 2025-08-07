using Unity.Entities;
using UnityEngine;
using UnityEngine.AI;

namespace RTS.Infantry
{
    /// <summary>
    /// Converts a GameObject with a NavMeshAgent into an infantry entity.
    /// Adds the <see cref="InfantryTag"/> and <see cref="NavAgent"/> components
    /// and stores the created entity on an attached <see cref="EntityReference"/>.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(NavMeshAgent))]
    public class InfantryAuthoring : MonoBehaviour
    {
        class Baker : Baker<InfantryAuthoring>
        {
            public override void Bake(InfantryAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                // Tag as infantry for system queries
                AddComponent<InfantryTag>(entity);

                // Wrap the NavMeshAgent in a managed component
                var agent = authoring.GetComponent<NavMeshAgent>();
                if (agent != null)
                {
                    AddComponentObject(entity, new NavAgent { Agent = agent });
                }

                // Cache the entity on the reference MonoBehaviour if present
                if (authoring.TryGetComponent<EntityReference>(out var reference))
                {
                    reference.Entity = entity;
                }
            }
        }
    }
}
