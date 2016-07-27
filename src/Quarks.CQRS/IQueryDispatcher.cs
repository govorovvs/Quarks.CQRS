using System.Threading;
using System.Threading.Tasks;

namespace Quarks.CQRS
{
	public interface IQueryDispatcher
	{
		Task<TResult> DispatchAsync<TQuery, TResult>(TQuery query) where TQuery : IQuery<TResult>;

		Task<TResult> DispatchAsync<TQuery, TResult>(TQuery query, CancellationToken ct) where TQuery : IQuery<TResult>;
	}
}