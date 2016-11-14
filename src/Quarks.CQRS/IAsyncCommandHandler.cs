using System.Threading;
using System.Threading.Tasks;

namespace Quarks.CQRS
{
    /// <summary>
    /// An object that handles the concrete command. 
    /// </summary>
    /// <typeparam name="TCommand">Type of command.</typeparam>
    public interface IAsyncCommandHandler<in TCommand> where TCommand : ICommand
    {
        /// <summary>
        /// Dispatches command to its handler.
        /// </summary>
        /// <param name="command">Command object.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous dispatch operation.</returns>
        Task HandleAsync(TCommand command, CancellationToken cancellationToken);
    }
}