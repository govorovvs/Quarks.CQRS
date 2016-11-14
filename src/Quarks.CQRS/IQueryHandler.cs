﻿namespace Quarks.CQRS
{
    /// <summary>
    ///  An object that handles the concrete query. 
    /// </summary>
    /// <typeparam name="TQuery">Type of query.</typeparam>
    /// <typeparam name="TResult">Type of result.</typeparam>
    public interface IQueryHandler<in TQuery, out TResult> where TQuery : IQuery
    {
        /// <summary>
        /// Handles query.
        /// </summary>
        /// <param name="query">Query object.</param>
        /// <returns>Result of the query.</returns>
        TResult Handle(TQuery query);
    }
}