using System.Threading;
using System.Threading.Tasks;

namespace Quarks.CQRS
{
	public interface ICommandHandler<in TCommand> where TCommand : ICommand
	{
		Task HandleAsync(TCommand command, CancellationToken ct);
	}
}