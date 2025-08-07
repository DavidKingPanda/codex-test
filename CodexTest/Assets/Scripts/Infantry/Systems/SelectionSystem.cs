using Unity.Entities;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RTS.Infantry
{
    /// <summary>
    /// Handles single and drag selection of infantry units.
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class SelectionSystem : SystemBase
    {
        private Camera _camera;
        private bool _isDragging;
        private Vector2 _dragStart;

        protected override void OnCreate()
        {
            _camera = Camera.main;
        }

        protected override void OnUpdate()
        {
            var mouse = Mouse.current;
            if (mouse == null || _camera == null) return;

            if (mouse.leftButton.wasPressedThisFrame)
            {
                _isDragging = true;
                _dragStart = mouse.position.ReadValue();
            }

            if (mouse.leftButton.wasReleasedThisFrame && _isDragging)
            {
                var end = mouse.position.ReadValue();
                if (Vector2.Distance(_dragStart, end) < 4f)
                {
                    SingleSelect(end);
                }
                else
                {
                    DragSelect(_dragStart, end);
                }

                _isDragging = false;
            }
        }

        private void SingleSelect(Vector2 screenPos)
        {
            var ray = _camera.ScreenPointToRay(screenPos);
            if (Physics.Raycast(ray, out var hit))
            {
                if (hit.transform.TryGetComponent<EntityReference>(out var reference))
                {
                    var entity = reference.Entity;
                    if (EntityManager.Exists(entity) && EntityManager.HasComponent<InfantryTag>(entity))
                    {
                        EntityManager.AddComponent<SelectedTag>(entity);
                    }
                }
            }
        }

        private void DragSelect(Vector2 start, Vector2 end)
        {
            // TODO: Implement marquee selection logic
        }
    }
}
