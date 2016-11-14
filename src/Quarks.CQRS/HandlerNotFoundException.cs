using System;

namespace Quarks.CQRS
{
    /// <summary>
    /// The exception that is thrown when a dispatcher attempts to resolve handler that is not available.
    /// </summary>
    public class HandlerNotFoundException : Exception
    {
        private HandlerNotFoundException(Type type)
            : base($"Handler for {type} is not found")
        {
        }

        internal static HandlerNotFoundException Command(ICommand command)
        {
            return new HandlerNotFoundException(command.GetType());
        }

        internal static HandlerNotFoundException Query(IQuery query)
        {
            return new HandlerNotFoundException(query.GetType());
        }
    }
}