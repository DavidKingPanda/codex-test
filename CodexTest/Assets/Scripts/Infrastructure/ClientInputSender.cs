using Game.Domain.Commands;
using Game.Domain.ECS;
using Game.Networking;
using Game.Networking.Messages;
using Game.Utils;
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
        private Transform _target;
        private float _tickAccumulator;
        private float _fixedDeltaTime;
        public Entity PlayerEntity { get; private set; }

        /// <summary>
        /// Injects dependencies from ClientBootstrap.
        /// </summary>
        public void Initialize(NetworkManager manager, Entity playerEntity, Transform target)
        {
            networkManager = manager;
            PlayerEntity = playerEntity;
            _target = target;
            _fixedDeltaTime = 1f / Constants.ServerTickRate;
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
            if (networkManager == null || !networkManager.IsConnected || _target == null)
                return;

            if (context.performed && Mathf.Abs(_verticalVelocity) < 0.01f && _target.position.y <= 0f)
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
            if (networkManager == null || !networkManager.IsConnected || _target == null)
                return;

            _tickAccumulator += Time.deltaTime;
            while (_tickAccumulator >= _fixedDeltaTime)
            {
                if (_input != Vector2.zero)
                {
                    var direction = new Vector3(_input.x, 0f, _input.y);

                    // Client-side prediction for horizontal movement.
                    _target.Translate(direction.normalized * moveSpeed * _fixedDeltaTime, Space.World);

                    var command = new MoveCommand(PlayerEntity, direction, moveSpeed);
                    var payload = JsonUtility.ToJson(command);
                    var message = new NetworkMessage(MessageType.MoveCommand, payload);
                    networkManager.SendMessage(message);
                }

                if (_verticalVelocity != 0f || _target.position.y > 0f)
                {
                    _verticalVelocity += gravity * _fixedDeltaTime;
                    var pos = _target.position;
                    pos.y += _verticalVelocity * _fixedDeltaTime;
                    if (pos.y < 0f)
                    {
                        pos.y = 0f;
                        _verticalVelocity = 0f;
                    }
                    _target.position = pos;
                }

                _tickAccumulator -= _fixedDeltaTime;
            }
        }
    }
}
