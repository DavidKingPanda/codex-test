using Game.Domain.Commands;
using Game.Infrastructure;

namespace Game.Domain
{
    /// <summary>
    /// Dispatches commands to systems via EventBus.
    /// </summary>
    public class CommandHandler
    {
        private readonly EventBus _eventBus;

        public CommandHandler(EventBus eventBus)
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
