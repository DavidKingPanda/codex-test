using System;
using Game.Domain.ECS;

namespace Game.Networking.Messages
{
    /// <summary>
    /// Message sent from server to client to inform about the spawned player entity.
    /// </summary>
    [Serializable]
    public struct SpawnPlayer
    {
        public int EntityId;
        /// <summary>
        /// Walk speed for local prediction.
        /// </summary>
        public float WalkSpeed;
        /// <summary>
        /// Run speed for local prediction.
        /// </summary>
        public float RunSpeed;
        /// <summary>
        /// Jump force for local prediction.
        /// </summary>
        public float JumpForce;
        /// <summary>
        /// Gravity for local prediction.
        /// </summary>
        public float Gravity;

        public SpawnPlayer(Entity entity, float walkSpeed, float runSpeed, float jumpForce, float gravity)
        {
            EntityId = entity.Id;
            WalkSpeed = walkSpeed;
            RunSpeed = runSpeed;
            JumpForce = jumpForce;
            Gravity = gravity;
        }
    }
}
