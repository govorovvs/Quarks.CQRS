using System;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;

namespace Quarks.CQRS.Impl
{
    /// <summary>
    /// An object that dispatches the concrete command to its handler.
    /// </summary>
    public class CommandDispatcher : CommandDispatcher<ICommand>
    {
        /// <summary>
        /// Initialize a new instance of <see cref="CommandDispatcher"/> class.
        /// </summary>
        /// <param name="commandHandlerResolver">An object that resolves query handlers.</param>
        public CommandDispatcher(ICommandHandlerResolver commandHandlerResolver) : base(commandHandlerResolver)
        {
        }
    }

    /// <summary>
    /// An object that dispatches the concrete command to its handler.
    /// </summary>
    /// <typeparam name="TBaseCommand">Type of command.</typeparam>
    public class CommandDispatcher<TBaseCommand> : ICommandDispatcher<TBaseCommand> where TBaseCommand : ICommand
    {
        private readonly ICommandHandlerResolver _commandHandlerResolver;

        /// <summary>
        /// Initialize a new instance of <see cref="CommandDispatcher{TBaseCommand}"/> class.
        /// </summary>
        /// <param name="commandHandlerResolver">An object that resolves query handlers.</param>
        public CommandDispatcher(ICommandHandlerResolver commandHandlerResolver)
        {
            _commandHandlerResolver = commandHandlerResolver;
        }

        /// <summary>
        /// Dispatches command to its handler.
        /// </summary>
        /// <param name="command">Command object.</param>
        public virtual void Dispatch(TBaseCommand command)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            if (TryDispatchSync(command))
            {
                return;
            }

            if (TryDispatchAsyncAsSync(command))
            {
                return;
            }

            throw HandlerNotFoundException.Command(command);
        }

        /// <summary>
        /// Asynchronously dispatches command to its handler.
        /// </summary>
        /// <param name="command">Command object.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous dispatch operation.</returns>
        public virtual Task DispatchAsync(TBaseCommand command, CancellationToken cancellationToken)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            Task task;
            if (TryDispatchAsync(command, cancellationToken, out task))
            {
                return task;
            }

            if (TryDispatchSync(command))
            {
                return Task.FromResult(0);
            }

            throw HandlerNotFoundException.Command(command);
        }

        private bool TryDispatchSync(TBaseCommand command)
        {
            Type handlerType =
               typeof(ICommandHandler<>).MakeGenericType(command.GetType());

            object handler;
            if (_commandHandlerResolver.TryResolveHandler(handlerType, out handler) == false)
            {
                return false;
            }

            Handle(handler, command);
            return true;
        }

        private bool TryDispatchAsync(TBaseCommand command, CancellationToken cancellationToken, out Task task)
        {
            Type handlerType =
               typeof(IAsyncCommandHandler<>).MakeGenericType(command.GetType());

            object handler;
            if (_commandHandlerResolver.TryResolveHandler(handlerType, out handler) == false)
            {
                task = null;
                return false;
            }

            task = HandleAsync(handler, command, cancellationToken);
            return true;
        }

        private bool TryDispatchAsyncAsSync(TBaseCommand command)
        {
            try
            {
                Task task;
                if (TryDispatchAsync(command, CancellationToken.None, out task))
                {
                    task.ConfigureAwait(false).GetAwaiter().GetResult();
                    return true;
                }
            }
            catch (AggregateException ex)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
            }

            return false;
        }

        private static void Handle(object handler, TBaseCommand command)
        {
            try
            {
                const string methodName =
                    nameof(ICommandHandler<TBaseCommand>.Handle);
                MethodInfo method = handler.GetType()
                    .GetRuntimeMethod(methodName, new[] { command.GetType() });
                method.Invoke(handler, new object[] { command });
            }
            catch (TargetInvocationException ex)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
            }
        }

        private static Task HandleAsync(object handler, TBaseCommand command, CancellationToken cancellationToken)
        {
            try
            {
                const string methodName =
                    nameof(IAsyncCommandHandler<TBaseCommand>.HandleAsync);
                MethodInfo method = handler.GetType()
                    .GetRuntimeMethod(methodName, new[] { command.GetType(), cancellationToken.GetType() });
                return (Task)method.Invoke(handler, new object[] { command, cancellationToken });
            }
            catch (TargetInvocationException ex)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
            }

            return null;
        }
    }
}