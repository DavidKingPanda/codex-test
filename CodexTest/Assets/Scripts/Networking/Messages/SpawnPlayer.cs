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

        public SpawnPlayer(Entity entity, float walkSpeed, float runSpeed)
        {
            EntityId = entity.Id;
            WalkSpeed = walkSpeed;
            RunSpeed = runSpeed;
        }
    }
}
