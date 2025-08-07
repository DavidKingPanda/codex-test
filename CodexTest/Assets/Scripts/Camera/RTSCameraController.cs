using UnityEngine;
using UnityEngine.InputSystem;

namespace RTS.CameraSystem
{
    /// <summary>
    /// Basic top-down RTS camera with pan, rotate and zoom controls.
    /// </summary>
    public class RTSCameraController : MonoBehaviour
    {
        public float moveSpeed = 10f;
        public float rotateSpeed = 100f;
        public float zoomSpeed = 100f;
        public float minZoom = 10f;
        public float maxZoom = 100f;

        private Camera _camera;

        private void Awake()
        {
            _camera = GetComponentInChildren<Camera>();
            if (_camera == null)
                _camera = Camera.main;
        }

        private void Update()
        {
            var keyboard = Keyboard.current;
            var mouse = Mouse.current;
            if (keyboard == null || mouse == null)
                return;

            Vector3 direction = Vector3.zero;
            if (keyboard.wKey.isPressed) direction += Vector3.forward;
            if (keyboard.sKey.isPressed) direction += Vector3.back;
            if (keyboard.aKey.isPressed) direction += Vector3.left;
            if (keyboard.dKey.isPressed) direction += Vector3.right;
            transform.Translate(direction * moveSpeed * Time.deltaTime, Space.World);

            if (mouse.middleButton.isPressed)
            {
                Vector2 delta = mouse.delta.ReadValue();
                transform.Rotate(Vector3.up, delta.x * rotateSpeed * Time.deltaTime, Space.World);
            }

            if (_camera != null)
            {
                float scroll = mouse.scroll.ReadValue().y;
                if (Mathf.Abs(scroll) > 0.01f)
                {
                    Vector3 pos = _camera.transform.localPosition;
                    pos.y = Mathf.Clamp(pos.y - scroll * zoomSpeed * Time.deltaTime, minZoom, maxZoom);
                    _camera.transform.localPosition = pos;
                }
            }
        }
    }
}
