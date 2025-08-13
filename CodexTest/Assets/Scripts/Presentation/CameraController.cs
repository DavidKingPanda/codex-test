using UnityEngine;

namespace Game.Presentation
{
    /// <summary>
    /// Camera controller that allows temporary panning with right mouse button.
    /// Presentation only; contains no gameplay logic.
    /// </summary>
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 offset = new Vector3(0f, 10f, -10f);
        [SerializeField] private float followSpeed = 5f;
        [SerializeField] private float dragSensitivity = 5f;
        [SerializeField] private float maxDragDistance = 5f;

        private Vector3 baseOffset;
        private Vector3 dragOffset;
        private Quaternion _initialRotation;

        private void Awake()
        {
            baseOffset = offset;
            _initialRotation = transform.rotation;
        }

        private void LateUpdate()
        {
            if (target == null) return;

            if (Input.GetMouseButton(1))
            {
                Vector2 mouse = Input.mousePosition;
                Vector2 screenSize = new Vector2(Screen.width, Screen.height);
                Vector2 normalized = (mouse / screenSize) - new Vector2(0.5f, 0.5f);

                Vector3 right = _initialRotation * Vector3.right;
                Vector3 forward = _initialRotation * Vector3.forward;
                right.y = 0f;
                forward.y = 0f;
                right.Normalize();
                forward.Normalize();

                Vector3 offset = (-normalized.x * right + -normalized.y * forward) * dragSensitivity;
                dragOffset = Vector3.ClampMagnitude(offset, maxDragDistance);
            }
            else
            {
                dragOffset = Vector3.Lerp(dragOffset, Vector3.zero, followSpeed * Time.deltaTime);
            }

            var desired = target.position + baseOffset + dragOffset;
            transform.position = Vector3.Lerp(transform.position, desired, followSpeed * Time.deltaTime);

            transform.rotation = _initialRotation;
        }

        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
        }
    }
}
