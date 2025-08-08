using Game.Components;
using Game.Domain.Commands;
using Game.Domain.ECS;
using Game.Infrastructure;

namespace Game.Systems
{
    /// <summary>
    /// Processes MoveCommand events and updates entity positions on the server.
    /// </summary>
    public class MovementSystem : ISystem
    {
        private readonly World _world;
        private readonly EventBus _eventBus;

        public MovementSystem(World world, EventBus eventBus)
        {
            _world = world;
            _eventBus = eventBus;
            _eventBus.Subscribe<MoveCommand>(OnMoveCommand);
        }

        public void Update(World world, float deltaTime)
        {
            // Movement is event driven; no per-frame logic required here.
        }

        private void OnMoveCommand(MoveCommand command)
        {
            if (_world.TryGetComponent(command.Entity, out PositionComponent position))
            {
                position.Value += command.Direction.normalized * command.Speed;
                _world.SetComponent(command.Entity, position);
            }
        }
    }
}
