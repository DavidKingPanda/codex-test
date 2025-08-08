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
        private Vector2 _input;
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

        private void Update()
        {
            if (_input == Vector2.zero)
                return;

            var direction = new Vector3(input.x, 0f, input.y);
            var command = new MoveCommand(PlayerEntity, direction, moveSpeed * Time.deltaTime);
            var payload = JsonUtility.ToJson(command);
            var message = new NetworkMessage(MessageType.MoveCommand, payload);
            networkManager.SendMessage(message);
        }
    }
}
