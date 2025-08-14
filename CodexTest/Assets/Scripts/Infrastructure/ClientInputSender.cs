using Game.Domain.Commands;
using Game.Domain.ECS;
using Game.Domain.Events;
using Game.Networking;
using Game.Networking.Messages;
using Game.Utils;
using GameEventBus = Game.EventBus.EventBus;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Infrastructure
{
    /// <summary>
    /// Captures player input on the client and sends it to the server.
    /// </summary>
    public class ClientInputSender : MonoBehaviour
    {
        private INetworkTransport networkManager;
        private GameEventBus eventBus;
        private float walkSpeed;
        private float runSpeed;
        [SerializeField] private float jumpForce = 5f;
        [SerializeField] private float gravity = -9.81f;
        private Vector2 _input;
        private float _verticalVelocity;
        private Transform _target;
        private float _tickAccumulator;
        private float _fixedDeltaTime;
        private float _currentStamina;
        private bool _forceWalk;
        public Entity PlayerEntity { get; private set; }

        /// <summary>
        /// Injects dependencies from ClientBootstrap.
        /// </summary>
        public void Initialize(INetworkTransport manager, GameEventBus eventBus, Entity playerEntity, Transform target, float walkSpeed, float runSpeed, float jumpForce, float gravity)
        {
            networkManager = manager;
            this.eventBus = eventBus;
            PlayerEntity = playerEntity;
            _target = target;
            _fixedDeltaTime = 1f / Constants.ServerTickRate;
            this.walkSpeed = walkSpeed;
            this.runSpeed = runSpeed;
            this.jumpForce = jumpForce;
            this.gravity = gravity;
            this.eventBus.Subscribe<StaminaChangedEvent>(OnStaminaChanged);
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

            if (context.performed && Mathf.Abs(_verticalVelocity) < 0.01f && _target.position.y <= 0.01f)
            {
                _verticalVelocity = jumpForce;
                var command = new JumpCommand(PlayerEntity);
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
                Transform cam = Camera.main.transform;
                Vector3 direction = Vector3.zero;
                if (_input != Vector2.zero)
                {
                    Vector3 camForward = cam.forward;
                    camForward.y = 0f;
                    camForward.Normalize();
                    Vector3 camRight = cam.right;
                    camRight.y = 0f;
                    camRight.Normalize();
                    direction = camRight * _input.x + camForward * _input.y;
                }

                bool wantsRun = Keyboard.current != null && Keyboard.current.leftShiftKey.isPressed;
                if (_forceWalk && Keyboard.current != null && !Keyboard.current.leftShiftKey.isPressed)
                {
                    _forceWalk = false;
                }
                bool isRunning = wantsRun && !_forceWalk && _currentStamina > 0f;
                float speed = isRunning ? runSpeed : walkSpeed;

                if (direction != Vector3.zero)
                {
                    // Client-side prediction for horizontal movement.
                    _target.Translate(direction.normalized * speed * _fixedDeltaTime, Space.World);
                }

                var command = new MoveCommand(PlayerEntity, direction, speed, isRunning);
                var payload = JsonUtility.ToJson(command);
                var message = new NetworkMessage(MessageType.MoveCommand, payload);
                networkManager.SendMessage(message);

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

        private void OnStaminaChanged(StaminaChangedEvent evt)
        {
            _currentStamina = evt.Current;
            if (_currentStamina <= 0f)
            {
                _forceWalk = true;
            }
        }

        private void OnDestroy()
        {
            if (eventBus != null)
            {
                eventBus.Unsubscribe<StaminaChangedEvent>(OnStaminaChanged);
            }
        }
    }
}
