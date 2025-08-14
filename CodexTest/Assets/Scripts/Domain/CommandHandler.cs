using Game.Domain.Commands;
using GameEventBus = Game.EventBus.EventBus;

namespace Game.Domain
{
    /// <summary>
    /// Dispatches commands to systems via EventBus.
    /// </summary>
    public class CommandHandler
    {
        private readonly GameEventBus _eventBus;

        public CommandHandler(GameEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        public void Handle(MoveCommand command)
        {
            _eventBus.Publish(command);
        }

        public void Handle(JumpCommand command)
        {
            _eventBus.Publish(command);
        }
    }
}
