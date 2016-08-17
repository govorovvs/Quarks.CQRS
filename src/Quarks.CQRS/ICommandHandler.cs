using System.Threading;
using System.Threading.Tasks;

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
		/// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
		/// <returns>A task that represents the asynchronous handle operation.</returns>
		Task HandleAsync(TCommand command, CancellationToken cancellationToken);
	}
}