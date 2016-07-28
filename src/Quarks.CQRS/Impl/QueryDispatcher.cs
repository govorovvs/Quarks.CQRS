using System;
using System.Reflection;
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

		public Task<TResult> DispatchAsync<TResult>(IQuery<TResult> query)
		{
			return DispatchAsync(query, CancellationToken.None);
		}

		public Task<TResult> DispatchAsync<TResult>(IQuery<TResult> query, CancellationToken ct)
		{
			if (query == null) throw new ArgumentNullException(nameof(query));

			Type queryType = query.GetType();
			object handler = ResolveHandler(queryType, typeof(TResult));
			MethodInfo method = handler.GetType().GetRuntimeMethod("HandleAsync", new[] {queryType, ct.GetType()});
			return (Task<TResult>) method.Invoke(handler, new object[] {query, ct});
		}

		private object ResolveHandler(Type queryType, Type resultType) 
		{
			Type handlerType = typeof(IQueryHandler<,>);
			Type type = handlerType.MakeGenericType(queryType, resultType);

			return _serviceProvider.GetService(type);
		}
	}
}