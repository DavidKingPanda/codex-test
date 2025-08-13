using UnityEngine;

namespace Game.Presentation
{
    /// <summary>
    /// Camera controller that allows temporary panning with right mouse button
    /// and orbital rotation with the middle mouse button.
    /// Presentation only; contains no gameplay logic.
    /// </summary>
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 offset = new Vector3(0f, 10f, -10f);
        [SerializeField] private float followSpeed = 5f;
        [SerializeField] private float dragSensitivity = 5f;
        [SerializeField] private float maxDragDistance = 5f;
        [SerializeField] private float orbitSensitivity = 0.2f;
        [SerializeField] private float hoverSensitivity = 1f;

        private Vector3 baseOffset;
        private Vector3 dragOffset;
        private Vector3 targetDragOffset;
        private Vector3 previousMousePosition;
        private Quaternion _initialRotation;
        private float _yaw;

        private void Awake()
        {
            baseOffset = offset;
            _initialRotation = transform.rotation;
        }

        private void LateUpdate()
        {
            if (target == null) return;

            if (Input.GetMouseButtonDown(2))
            {
                previousMousePosition = Input.mousePosition;
            }

            if (Input.GetMouseButton(2))
            {
                Vector3 currentMouse = Input.mousePosition;
                float deltaX = currentMouse.x - previousMousePosition.x;
                _yaw += deltaX * orbitSensitivity;
                previousMousePosition = currentMouse;
            }

            var rotation = Quaternion.Euler(0f, _yaw, 0f) * _initialRotation;

            Vector2 mouse = Input.mousePosition;
            Vector2 screenSize = new Vector2(Screen.width, Screen.height);
            Vector2 normalized = ((mouse / screenSize) - new Vector2(0.5f, 0.5f)) * 2f;

            Vector3 right = rotation * Vector3.right;
            Vector3 forward = rotation * Vector3.forward;
            right.y = 0f;
            forward.y = 0f;
            right.Normalize();
            forward.Normalize();

            Vector3 hoverOffset = (normalized.x * right + normalized.y * forward) * hoverSensitivity;

            if (Input.GetMouseButton(1))
            {
                Vector3 drag = (normalized.x * right + normalized.y * forward) * dragSensitivity;
                targetDragOffset = Vector3.ClampMagnitude(drag, maxDragDistance);
            }
            else
            {
                targetDragOffset = Vector3.zero;
            }

            dragOffset = Vector3.Lerp(dragOffset, targetDragOffset, followSpeed * Time.deltaTime);

            var desired = target.position + rotation * baseOffset + hoverOffset + dragOffset;
            transform.position = Vector3.Lerp(transform.position, desired, followSpeed * Time.deltaTime);

            transform.rotation = rotation;
        }

        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
        }
    }
}
