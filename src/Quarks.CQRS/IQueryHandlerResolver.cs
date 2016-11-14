using System;

namespace Quarks.CQRS
{
    /// <summary>
    /// An object that resolves query handlers.
    /// </summary>
    public interface IQueryHandlerResolver
    {
        /// <summary>
        /// Tries to resolve query handler of the specified type.
        /// </summary>
        /// <param name="handlerType">Type of query handler.</param>
        /// <param name="handler">Query handler object.</param>
        /// <returns>True - if resolve is creation success, otherwise - false.</returns>
        bool TryResolveHandler(Type handlerType, out object handler);
    }
}