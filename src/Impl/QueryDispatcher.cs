using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Quarks.CQRS.Impl
{
	/// <summary>
	/// Basic implementation of <see cref="IQueryDispatcher"/>. 
	/// </summary>
	public class QueryDispatcher : IQueryDispatcher
    {
        private readonly IServiceProvider _serviceProvider;

        private static readonly IDictionary<Type, MethodInfo> HandleMethods =
            new ConcurrentDictionary<Type, MethodInfo>();

        /// <summary>
		/// Initializes a new instance of <see cref="QueryDispatcher"/> class with handler factory.
		/// </summary>
		/// <param name="serviceProvider">An object that creates handlers.</param>
		public QueryDispatcher(IServiceProvider serviceProvider)
		{
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
		}

		/// <summary>
		/// Dispatches query to its handler.
		/// </summary>
		/// <typeparam name="TResult">Type of result.</typeparam>
		/// <param name="query">Query object.</param>
		/// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
		/// <returns>A task that represents the asynchronous dispatch operation.</returns>
		public Task<TResult> DispatchAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken)
		{
			if (query == null) throw new ArgumentNullException(nameof(query));

			Type queryType = query.GetType();
			object handler = ResolveHandler(queryType, typeof(TResult));

            if (HandleMethods.TryGetValue(handler.GetType(), out var method) == false)
            {
				method = handler.GetType().GetRuntimeMethod("HandleAsync", new[] { queryType, cancellationToken.GetType() });
                HandleMethods.Add(handler.GetType(), method);
            }

			return (Task<TResult>) method.Invoke(handler, new object[] {query, cancellationToken});
		}

		private object ResolveHandler(Type queryType, Type resultType) 
		{
            Type handlerType = typeof(IQueryHandler<,>);
			Type type = handlerType.MakeGenericType(queryType, resultType);
			return _serviceProvider.GetService(type);
        }
	}
}