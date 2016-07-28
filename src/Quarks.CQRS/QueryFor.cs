using System.Threading;
using System.Threading.Tasks;

namespace Quarks.CQRS
{
	public class QueryFor<TResult>
	{
		private readonly IQueryDispatcher _queryDispatcher;

		public QueryFor(IQueryDispatcher queryDispatcher)
		{
			_queryDispatcher = queryDispatcher;
		}

		public Task<TResult> DispatchAsync<TQuery>(TQuery query) where TQuery : IQuery<TResult>
		{
			return DispatchAsync(query, CancellationToken.None);
		}

		public Task<TResult> DispatchAsync<TQuery>(TQuery query, CancellationToken ct) where TQuery : IQuery<TResult>
		{
			return _queryDispatcher.DispatchAsync(query, ct);
		} 
	}
}