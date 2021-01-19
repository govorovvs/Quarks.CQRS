using System;
using System.Threading;
using System.Threading.Tasks;

namespace Quarks.CQRS.Impl
{
	/// <summary>
	/// Basic implementation of <see cref="ICommandDispatcher"/>. 
	/// </summary>
	public class CommandDispatcher : ICommandDispatcher
	{
		private readonly ICommandHandlerFactory _commandHandlerFactory;

		/// <summary>
		/// Initializes a new instance of <see cref="CommandDispatcher"/> class with handler factory.
		/// </summary>
		/// <param name="commandHandlerFactory">An object that creates handlers.</param>
		public CommandDispatcher(ICommandHandlerFactory commandHandlerFactory)
		{
			if (commandHandlerFactory == null) throw new ArgumentNullException(nameof(commandHandlerFactory));

			_commandHandlerFactory = commandHandlerFactory;
		}

		/// <summary>
		/// Dispatches command to its handler.
		/// </summary>
		/// <typeparam name="TCommand">Type of command.</typeparam>
		/// <param name="command">Command object.</param>
		/// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
		/// <returns>A task that represents the asynchronous dispatch operation.</returns>
		public Task DispatchAsync<TCommand>(TCommand command, CancellationToken cancellationToken) where TCommand : ICommand
		{
			if (command == null) throw new ArgumentNullException(nameof(command));

			ICommandHandler<TCommand> handler = ResolveHandler<TCommand>();
			return handler.HandleAsync(command, cancellationToken);
		}

		private ICommandHandler<TCommand> ResolveHandler<TCommand>() where TCommand : ICommand
		{
			return (ICommandHandler<TCommand>) _commandHandlerFactory.CreateHandler(typeof (ICommandHandler<TCommand>));
		} 
	}
}