using System;
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
		private Mock<IServiceProvider> _mockServiceProvider;
		private CommandDispatcher _dispatcher;
		private CancellationToken _cancellationToken;

		[SetUp]
		public void SetUp()
		{
			_cancellationToken = CancellationToken.None;
			_mockServiceProvider = new Mock<IServiceProvider>();
			_dispatcher = new CommandDispatcher(_mockServiceProvider.Object);
		}

		[Test]
		public async Task Dispatch_Resolves_Handler_And_Calls_It()
		{
			var fakeCommand = new FakeCommand();

			var handler = new Mock<ICommandHandler<FakeCommand>>();
			handler
				.Setup(x => x.HandleAsync(fakeCommand, _cancellationToken))
				.Returns(Task.CompletedTask);

			_mockServiceProvider
				.Setup(x => x.GetService(typeof(ICommandHandler<FakeCommand>)))
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

			_mockServiceProvider
				.Setup(x => x.GetService(typeof(ICommandHandler<FakeCommand>)))
				.Returns(handler.Object);

			await _dispatcher.DispatchAsync(fakeCommand);

			handler.VerifyAll();
		}

		public class FakeCommand : ICommand
		{
			
		}
	}
}