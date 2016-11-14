using System;

namespace Quarks.CQRS
{
    /// <summary>
    /// An object that resolves command handlers.
    /// </summary>
    public interface ICommandHandlerResolver
    {
        /// <summary>
        /// Tries to resolve command handler of the specified type.
        /// </summary>
        /// <param name="handlerType">Type of command handler.</param>
        /// <param name="handler">Command handler object.</param>
        /// <returns>True - if resolve is success, otherwise - false.</returns>
        bool TryResolveHandler(Type handlerType, out object handler);
    }
}