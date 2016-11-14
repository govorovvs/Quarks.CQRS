using System;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;

namespace Quarks.CQRS.Impl
{
    /// <summary>
    /// An object that dispatches the concrete query to its handler.
    /// </summary>
    public class QueryDispatcher : QueryDispatcher<IQuery>
    {
        /// <summary>
        /// Initialize a new instance of <see cref="QueryDispatcher"/> class with handler resolver.
        /// </summary>
        /// <param name="queryHandlerResolver">An object that resolves query handlers.</param>
        public QueryDispatcher(IQueryHandlerResolver queryHandlerResolver) : base(queryHandlerResolver)
        {
        }
    }

    /// <summary>
    /// An object that dispatches the concrete query to its handler.
    /// </summary>
    /// <typeparam name="TBaseQuery">Type of queries.</typeparam>
    public class QueryDispatcher<TBaseQuery> : IQueryDispatcher<TBaseQuery> where TBaseQuery : IQuery
    {
        private readonly IQueryHandlerResolver _queryHandlerResolver;

        /// <summary>
        /// Initialize a new instance of <see cref="QueryDispatcher{TQuery}"/> class  with handler resolver.
        /// </summary>
        /// <param name="queryHandlerResolver">An object that resolves query handlers.</param>
        public QueryDispatcher(IQueryHandlerResolver queryHandlerResolver)
        {
            _queryHandlerResolver = queryHandlerResolver;
        }

        /// <summary>
        /// Dispatches query to its handler.
        /// </summary>
        /// <typeparam name="TResult">Type of result.</typeparam>
        /// <param name="query">Query object.</param>
        /// <returns>Result of the query.</returns>
        public virtual TResult Dispatch<TResult>(TBaseQuery query)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            TResult result;
            if (TryDispatchSync(query, out result))
            {
                return result;
            }

            if (TryDispatchAsyncAsSync(query, out result))
            {
                return result;
            }

            throw HandlerNotFoundException.Query(query);
        }

        /// <summary>
        /// Asynchronously dispatches query to its handler.
        /// </summary>
        /// <typeparam name="TResult">Type of result.</typeparam>
        /// <param name="query">Query object.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous dispatch operation.</returns>
        public virtual Task<TResult> DispatchAsync<TResult>(TBaseQuery query, CancellationToken cancellationToken)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            Task<TResult> task;
            if (TryDispatchAsync(query, cancellationToken, out task))
            {
                return task;
            }

            TResult result;
            if (TryDispatchSync(query, out result))
            {
                return Task.FromResult(result);
            }

            throw HandlerNotFoundException.Query(query);
        }

        private bool TryDispatchSync<TResult>(TBaseQuery query, out TResult result)
        {
            Type handlerType =
                typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResult));

            object handler;
            if (_queryHandlerResolver.TryResolveHandler(handlerType, out handler) == false)
            {
                result = default(TResult);
                return false;
            }

            result = Handle<TResult>(handler, query);
            return true;
        }

        private bool TryDispatchAsync<TResult>(
            TBaseQuery query, CancellationToken cancellationToken, out Task<TResult> result)
        {
            Type handlerType =
                typeof(IAsyncQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResult));

            object handler;
            if (_queryHandlerResolver.TryResolveHandler(handlerType, out handler) == false)
            {
                result = null;
                return false;
            }

            result = HandleAsync<TResult>(handler, query, cancellationToken);
            return true;
        }

        private bool TryDispatchAsyncAsSync<TResult>(TBaseQuery query, out TResult result)
        {
            try
            {
                Task<TResult> task;
                if (TryDispatchAsync(query, CancellationToken.None, out task))
                {
                    result = task.ConfigureAwait(false).GetAwaiter().GetResult();
                    return true;
                }
            }
            catch (AggregateException ex)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
            }

            result = default(TResult);
            return false;
        }

        private static TResult Handle<TResult>(object handler, TBaseQuery query)
        {
            try
            {
                const string methodName =
                    nameof(IQueryHandler<TBaseQuery, TResult>.Handle);
                MethodInfo method = handler.GetType()
                    .GetRuntimeMethod(methodName, new[] { query.GetType() });
                return (TResult)method.Invoke(handler, new object[] { query });
            }
            catch (TargetInvocationException ex)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
            }

            return default(TResult);
        }

        private static Task<TResult> HandleAsync<TResult>(object handler, TBaseQuery query, CancellationToken cancellationToken)
        {
            try
            {
                const string methodName =
                    nameof(IAsyncQueryHandler<TBaseQuery, TResult>.HandleAsync);
                MethodInfo method = handler.GetType()
                    .GetRuntimeMethod(methodName, new[] { query.GetType(), cancellationToken.GetType() });
                return (Task<TResult>)method.Invoke(handler, new object[] { query, cancellationToken });
            }
            catch (TargetInvocationException ex)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
            }

            return null;
        }
    }
}