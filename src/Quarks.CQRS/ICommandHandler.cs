namespace Quarks.CQRS
{
    /// <summary>
    /// An object that handles the concrete command. 
    /// </summary>
    /// <typeparam name="TCommand">Type of command.</typeparam>
    public interface ICommandHandler<in TCommand> where TCommand : ICommand
    {
        /// <summary>
        /// Handles command.
        /// </summary>
        /// <param name="command">Query object.</param>
        void Handle(TCommand command);
    }
}