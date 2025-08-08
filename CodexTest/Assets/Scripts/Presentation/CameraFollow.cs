using UnityEngine;

namespace Game.Presentation
{
    /// <summary>
    /// Simple top-down camera that follows a target transform.
    /// Presentation only; no gameplay logic.
    /// </summary>
    public class CameraFollow : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 offset = new Vector3(0, 10, -10);
        [SerializeField] private float followSpeed = 5f;

        private void LateUpdate()
        {
            if (target == null) return;
            var desired = target.position + offset;
            transform.position = Vector3.Lerp(transform.position, desired, followSpeed * Time.deltaTime);
            transform.LookAt(target);
        }

        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
        }
    }
}
