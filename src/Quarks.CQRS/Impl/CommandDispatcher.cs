using System;
using System.Threading;
using System.Threading.Tasks;

namespace Quarks.CQRS.Impl
{
	public class CommandDispatcher : ICommandDispatcher
	{
		private readonly IHandlerFactory _handlerFactory;

		public CommandDispatcher(IHandlerFactory handlerFactory)
		{
			_handlerFactory = handlerFactory;
		}

		public Task DispatchAsync<TCommand>(TCommand command) where TCommand : ICommand
		{
			return DispatchAsync(command, CancellationToken.None);
		}

		public Task DispatchAsync<TCommand>(TCommand command, CancellationToken ct) where TCommand : ICommand
		{
			if (command == null) throw new ArgumentNullException(nameof(command));

			ICommandHandler<TCommand> handler = ResolveHandler<TCommand>();
			return handler.HandleAsync(command, ct);
		}

		private ICommandHandler<TCommand> ResolveHandler<TCommand>() where TCommand : ICommand
		{
			return (ICommandHandler<TCommand>) _handlerFactory.CreateHandler(typeof (ICommandHandler<TCommand>));
		} 
	}
}