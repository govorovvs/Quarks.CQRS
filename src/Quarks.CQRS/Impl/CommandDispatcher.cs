using System;
using System.Threading;
using System.Threading.Tasks;

namespace Quarks.CQRS.Impl
{
	public class CommandDispatcher : ICommandDispatcher
	{
		private readonly IServiceProvider _serviceProvider;

		public CommandDispatcher(IServiceProvider serviceProvider)
		{
			_serviceProvider = serviceProvider;
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
			return (ICommandHandler<TCommand>) _serviceProvider.GetService(typeof (ICommandHandler<TCommand>));
		} 
	}
}