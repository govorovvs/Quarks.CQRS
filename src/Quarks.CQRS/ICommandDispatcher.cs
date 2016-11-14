using System.Threading;
using System.Threading.Tasks;

namespace Quarks.CQRS
{
    /// <summary>
    /// An object that dispatches the concrete command to its handler.
    /// </summary>
    public interface ICommandDispatcher : ICommandDispatcher<ICommand>
    {
    }

    /// <summary>
    /// An object that dispatches the concrete command to its handler.
    /// </summary>
    /// <typeparam name="TBaseCommand">Type of command.</typeparam>
    public interface ICommandDispatcher<in TBaseCommand>
        where TBaseCommand : ICommand
    {
        /// <summary>
        /// Dispatches command to its handler.
        /// </summary>
        /// <param name="command">Command object.</param>
        void Dispatch(TBaseCommand command);

        /// <summary>
        /// Asynchronously dispatches command to its handler.
        /// </summary>
        /// <param name="command">Command object.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous dispatch operation.</returns>
        Task DispatchAsync(TBaseCommand command, CancellationToken cancellationToken = default(CancellationToken));
    }
}