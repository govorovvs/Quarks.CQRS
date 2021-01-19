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
        private readonly IServiceProvider _serviceProvider;

		/// <summary>
		/// Initializes a new instance of <see cref="CommandDispatcher"/> class with handler factory.
		/// </summary>
		/// <param name="serviceProvider">An object that creates handlers.</param>
		public CommandDispatcher(IServiceProvider serviceProvider)
		{
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
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

			var handler = ResolveHandler<TCommand>();
			return handler.HandleAsync(command, cancellationToken);
		}

		private ICommandHandler<TCommand> ResolveHandler<TCommand>() where TCommand : ICommand
        {
            return (ICommandHandler<TCommand>)_serviceProvider.GetService(typeof(ICommandHandler<TCommand>));
        } 
	}
}