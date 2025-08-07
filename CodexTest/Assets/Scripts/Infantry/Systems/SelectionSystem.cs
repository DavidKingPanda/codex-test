using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
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

        private void ClearSelection()
        {
            Entities
                .WithAll<SelectedTag>()
                .WithStructuralChanges()
                .ForEach((Entity entity) =>
                {
                    EntityManager.RemoveComponent<SelectedTag>(entity);
                }).Run();
        }

        private void SingleSelect(Vector2 screenPos)
        {
            var keyboard = Keyboard.current;
            bool additive = keyboard != null && keyboard.shiftKey.isPressed;
            if (!additive)
                ClearSelection();

            var ray = _camera.ScreenPointToRay(screenPos);
            if (Physics.Raycast(ray, out var hit))
            {
                var reference = hit.transform.GetComponentInParent<EntityReference>();
                if (reference != null)
                {
                    var entity = reference.Entity;
                    if (EntityManager.Exists(entity) && EntityManager.HasComponent<InfantryTag>(entity))
                    {
                        if (!EntityManager.HasComponent<SelectedTag>(entity))
                            EntityManager.AddComponent<SelectedTag>(entity);
                    }
                }
            }
        }

        private void DragSelect(Vector2 start, Vector2 end)
        {
            var keyboard = Keyboard.current;
            bool additive = keyboard != null && keyboard.shiftKey.isPressed;
            if (!additive)
                ClearSelection();

            var rect = Rect.MinMaxRect(
                math.min(start.x, end.x),
                math.min(start.y, end.y),
                math.max(start.x, end.x),
                math.max(start.y, end.y));

            Entities
                .WithAll<InfantryTag>()
                .WithStructuralChanges()
                .ForEach((Entity entity, in LocalToWorld transform) =>
                {
                    Vector3 screen = _camera.WorldToScreenPoint(transform.Position);
                    if (rect.Contains((Vector2)screen))
                    {
                        if (!EntityManager.HasComponent<SelectedTag>(entity))
                            EntityManager.AddComponent<SelectedTag>(entity);
                    }
                }).Run();
        }
    }
}
