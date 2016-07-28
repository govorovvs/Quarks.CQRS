using System.Threading;
using System.Threading.Tasks;

namespace Quarks.CQRS
{
	public interface IQueryDispatcher
	{
		Task<TResult> DispatchAsync<TResult>(IQuery<TResult> query);

		Task<TResult> DispatchAsync<TResult>(IQuery<TResult> query, CancellationToken ct);
	}
}