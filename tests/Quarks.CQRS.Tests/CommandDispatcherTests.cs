using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Quarks.CQRS.Impl;

namespace Quarks.CQRS.Tests
{
	[TestFixture]
	public class CommandDispatcherTests
	{
		private Mock<IHandlerFactory> _mockHandlerFactory;
		private CommandDispatcher _dispatcher;
		private CancellationToken _cancellationToken;

		[SetUp]
		public void SetUp()
		{
			_cancellationToken = CancellationToken.None;
			_mockHandlerFactory = new Mock<IHandlerFactory>();
			_dispatcher = new CommandDispatcher(_mockHandlerFactory.Object);
		}

		[Test]
		public async Task Dispatch_Resolves_Handler_And_Calls_It()
		{
			var fakeCommand = new FakeCommand();

			var handler = new Mock<ICommandHandler<FakeCommand>>();
			handler
				.Setup(x => x.HandleAsync(fakeCommand, _cancellationToken))
				.Returns(Task.CompletedTask);

			_mockHandlerFactory
				.Setup(x => x.CreateHandler(typeof(ICommandHandler<FakeCommand>)))
				.Returns(handler.Object);

			await _dispatcher.DispatchAsync(fakeCommand, _cancellationToken);

			handler.VerifyAll();
		}

		[Test]
		public async Task Dispatch_With_No_CancellationToken_Resolves_Handler_And_Calls_It()
		{
			var fakeCommand = new FakeCommand();

			var handler = new Mock<ICommandHandler<FakeCommand>>();
			handler
				.Setup(x => x.HandleAsync(fakeCommand, CancellationToken.None))
				.Returns(Task.CompletedTask);

			_mockHandlerFactory
				.Setup(x => x.CreateHandler(typeof(ICommandHandler<FakeCommand>)))
				.Returns(handler.Object);

			await _dispatcher.DispatchAsync(fakeCommand);

			handler.VerifyAll();
		}

		public class FakeCommand : ICommand
		{
			
		}
	}
}