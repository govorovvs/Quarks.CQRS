using System.Threading;
using System.Threading.Tasks;

namespace Quarks.CQRS
{
	public interface ICommandDispatcher
	{
		Task DispatchAsync<TCommand>(TCommand command) where TCommand : ICommand;

		Task DispatchAsync<TCommand>(TCommand command, CancellationToken ct) where TCommand : ICommand;
	}
}