using System.Threading;
using System.Threading.Tasks;

namespace Quarks.CQRS
{
    /// <summary>
    ///  An object that handles the concrete query. 
    /// </summary>
    /// <typeparam name="TQuery">Type of query.</typeparam>
    /// <typeparam name="TResult">Type of result.</typeparam>
    public interface IAsyncQueryHandler<in TQuery, TResult> where TQuery : IQuery
    {
        /// <summary>
        /// Handles query.
        /// </summary>
        /// <param name="query">Query object.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>Result of the query.</returns>
        Task<TResult> HandleAsync(TQuery query, CancellationToken cancellationToken);
    }
}