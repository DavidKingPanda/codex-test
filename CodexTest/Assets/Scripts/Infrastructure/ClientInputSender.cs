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
        [SerializeField] private NetworkManager networkManager;
        [SerializeField] private float moveSpeed = 5f;
        private Vector2 moveInput;
        public Entity PlayerEntity { get; set; }

        public void OnMove(InputAction.CallbackContext context)
        {
            moveInput = context.ReadValue<Vector2>();
        }

        private void Update()
        {
            if (moveInput == Vector2.zero)
                return;
            var direction = new Vector3(moveInput.x, 0, moveInput.y);
            var command = new MoveCommand(PlayerEntity, direction, moveSpeed * Time.deltaTime);
            networkManager.SendMessage(command);
        }
    }
}
