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
        [SerializeField] private float dragSensitivity = 0.5f;
        [SerializeField] private float maxDragDistance = 5f;

        private Vector3 baseOffset;
        private Vector3 dragOffset;

        private void Awake()
        {
            baseOffset = offset;
        }

        private void LateUpdate()
        {
            if (target == null) return;

            if (Input.GetMouseButton(1))
            {
                float h = -Input.GetAxis("Mouse X") * dragSensitivity;
                float v = -Input.GetAxis("Mouse Y") * dragSensitivity;
                var delta = new Vector3(h, 0f, v);

                dragOffset += delta;
                dragOffset = Vector3.ClampMagnitude(dragOffset, maxDragDistance);
                transform.position = target.position + baseOffset + dragOffset;
            }
            else
            {
                dragOffset = Vector3.Lerp(dragOffset, Vector3.zero, followSpeed * Time.deltaTime);
                var desired = target.position + baseOffset + dragOffset;
                transform.position = Vector3.Lerp(transform.position, desired, followSpeed * Time.deltaTime);
            }

            transform.LookAt(target);
        }

        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
        }
    }
}
