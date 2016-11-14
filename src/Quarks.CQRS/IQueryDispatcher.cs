using System.Threading;
using System.Threading.Tasks;

namespace Quarks.CQRS
{
    /// <summary>
    /// An object that dispatches the concrete query to its handler.
    /// </summary>
    public interface IQueryDispatcher : IQueryDispatcher<IQuery>
    {
    }

    /// <summary>
    /// An object that dispatches the concrete query to its handler.
    /// </summary>
    /// <typeparam name="TBaseQuery">Type of queries.</typeparam>
    public interface IQueryDispatcher<in TBaseQuery> where TBaseQuery : IQuery
    {
        /// <summary>
        /// Dispatches query to its handler.
        /// </summary>
        /// <typeparam name="TResult">Type of result.</typeparam>
        /// <param name="query">Query object.</param>
        /// <returns>Result of the query.</returns>
        TResult Dispatch<TResult>(TBaseQuery query);

        /// <summary>
        /// Asynchronously dispatches query to its handler.
        /// </summary>
        /// <typeparam name="TResult">Type of result.</typeparam>
        /// <param name="query">Query object.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous dispatch operation.</returns>
        Task<TResult> DispatchAsync<TResult>(TBaseQuery query, CancellationToken cancellationToken = default(CancellationToken));
    }
}