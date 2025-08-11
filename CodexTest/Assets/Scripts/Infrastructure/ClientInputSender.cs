using Game.Domain.Commands;
using Game.Domain.ECS;
using Game.Networking;
using Game.Networking.Messages;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Infrastructure
{
    /// <summary>
    /// Captures player input on the client and sends it to the server.
    /// </summary>
    public class ClientInputSender : MonoBehaviour
    {
        private NetworkManager networkManager;
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float jumpForce = 5f;
        [SerializeField] private float gravity = -9.81f;
        private Vector2 _input;
        private float _verticalVelocity;
        public Entity PlayerEntity { get; private set; }

        /// <summary>
        /// Injects dependencies from ClientBootstrap.
        /// </summary>
        public void Initialize(NetworkManager manager, Entity playerEntity)
        {
            networkManager = manager;
            PlayerEntity = playerEntity;
        }

        /// <summary>
        /// Called by Unity's Input System when the Move action is triggered.
        /// Stores the current 2D input direction. Actual commands are sent
        /// each frame from Update for smooth continuous movement.
        /// </summary>
        public void OnMove(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                _input = context.ReadValue<Vector2>();
            }
            else if (context.canceled)
            {
                _input = Vector2.zero;
            }
        }

        /// <summary>
        /// Called when the Jump action is triggered.
        /// Sends a jump command to the server and applies local prediction.
        /// </summary>
        public void OnJump(InputAction.CallbackContext context)
        {
            if (context.performed && Mathf.Abs(_verticalVelocity) < 0.01f)
            {
                _verticalVelocity = jumpForce;
                var command = new JumpCommand(PlayerEntity, jumpForce);
                var payload = JsonUtility.ToJson(command);
                var message = new NetworkMessage(MessageType.JumpCommand, payload);
                networkManager.SendMessage(message);
            }
        }

        private void Update()
        {
            if (_input != Vector2.zero)
            {
                var direction = new Vector3(_input.x, 0f, _input.y);

                // Client-side prediction for horizontal movement.
                transform.Translate(direction.normalized * moveSpeed * Time.deltaTime, Space.World);

                var command = new MoveCommand(PlayerEntity, direction, moveSpeed);
                var payload = JsonUtility.ToJson(command);
                var message = new NetworkMessage(MessageType.MoveCommand, payload);
                networkManager.SendMessage(message);
            }

            if (_verticalVelocity != 0f || transform.position.y > 0f)
            {
                _verticalVelocity += gravity * Time.deltaTime;
                var pos = transform.position;
                pos.y += _verticalVelocity * Time.deltaTime;
                if (pos.y < 0f)
                {
                    pos.y = 0f;
                    _verticalVelocity = 0f;
                }
                transform.position = pos;
            }
        }
    }
}
