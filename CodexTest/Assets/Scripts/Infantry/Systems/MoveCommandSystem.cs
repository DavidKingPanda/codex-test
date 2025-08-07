using Unity.Entities;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RTS.Infantry
{
    /// <summary>
    /// Issues move orders to selected units based on right mouse clicks.
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class MoveCommandSystem : SystemBase
    {
        private Camera _camera;

        protected override void OnCreate()
        {
            _camera = Camera.main;
        }

        protected override void OnUpdate()
        {
            var mouse = Mouse.current;
            if (mouse == null || _camera == null || !mouse.rightButton.wasPressedThisFrame)
                return;

            var ray = _camera.ScreenPointToRay(mouse.position.ReadValue());
            if (Physics.Raycast(ray, out var hit))
            {
                var destination = hit.point;

                Entities
                    .WithAll<SelectedTag>()
                    .WithStructuralChanges()
                    .ForEach((Entity entity) =>
                    {
                        if (EntityManager.HasComponent<MoveTarget>(entity))
                        {
                            EntityManager.SetComponentData(entity, new MoveTarget { Position = destination });
                        }
                        else
                        {
                            EntityManager.AddComponentData(entity, new MoveTarget { Position = destination });
                        }
                    }).Run();
            }
        }
    }
}
