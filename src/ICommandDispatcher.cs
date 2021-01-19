using System.Threading;
using System.Threading.Tasks;

namespace Quarks.CQRS
{
	/// <summary>
	/// An object that dispatches the concrete command to its handler.
	/// </summary>
	public interface ICommandDispatcher
	{
		/// <summary>
		/// Dispatches command to its handler.
		/// </summary>
		/// <typeparam name="TCommand">Type of command.</typeparam>
		/// <param name="command">Command object.</param>
		/// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
		/// <returns>A task that represents the asynchronous dispatch operation.</returns>
		Task DispatchAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default(CancellationToken)) where TCommand : ICommand;
	}
}