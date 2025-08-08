using Game.Domain.Commands;
using Game.Domain.ECS;
using Game.Networking;
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
        /// Translates the 2D input into a 3D direction and sends a MoveCommand
        /// to the server. Movement occurs only on the server.
        /// </summary>
        public void OnMove(InputAction.CallbackContext context)
        {
            if (!context.performed)
                return;
            Vector2 input = context.ReadValue<Vector2>();
            if (input == Vector2.zero)
                return;

            var direction = new Vector3(input.x, 0f, input.y);
            var command = new MoveCommand(PlayerEntity, direction, moveSpeed * Time.deltaTime);
            networkManager.SendMessage(command);
        }
    }
}
