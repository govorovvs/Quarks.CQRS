using System;
using System.Threading;
using System.Threading.Tasks;

namespace Quarks.CQRS.Impl
{
	public class QueryDispatcher : IQueryDispatcher
	{
		private readonly IServiceProvider _serviceProvider;

		public QueryDispatcher(IServiceProvider serviceProvider)
		{
			_serviceProvider = serviceProvider;
		}

		public Task<TResult> DispatchAsync<TQuery, TResult>(TQuery query) where TQuery : IQuery<TResult>
		{
			return DispatchAsync<TQuery, TResult>(query, CancellationToken.None);
		}

		public Task<TResult> DispatchAsync<TQuery, TResult>(TQuery query, CancellationToken ct) where TQuery : IQuery<TResult>
		{
			if (query == null) throw new ArgumentNullException(nameof(query));

			IQueryHandler<TQuery, TResult> handler = ResolveHandler<TQuery, TResult>();
			return handler.HandleAsync(query, ct);
		}

		private IQueryHandler<TQuery, TResult> ResolveHandler<TQuery, TResult>() where TQuery : IQuery<TResult>
		{
			return (IQueryHandler<TQuery, TResult>)_serviceProvider.GetService(typeof(IQueryHandler<TQuery,TResult>));
		}
	}
}