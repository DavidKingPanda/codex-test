using Unity.Entities;
using UnityEngine;

namespace RTS.Infantry
{
    /// <summary>
    /// Optional authoring that marks the unit as selected at bake time
    /// by adding the <see cref="SelectedTag"/> component.
    /// Attach this script to a GameObject to start it selected.
    /// </summary>
    [DisallowMultipleComponent]
    public class SelectedAuthoring : MonoBehaviour
    {
        class Baker : Baker<SelectedAuthoring>
        {
            public override void Bake(SelectedAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<SelectedTag>(entity);
            }
        }
    }
}
