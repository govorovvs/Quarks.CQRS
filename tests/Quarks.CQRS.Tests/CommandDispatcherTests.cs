using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Moq.Language.Flow;
using NUnit.Framework;
using Quarks.CQRS.Impl;

namespace Quarks.CQRS.Tests
{
    [TestFixture]
    public class CommandDispatcherTests
    {
        private Mock<ICommandHandlerResolver> _mockResolver;
        private Mock<ICommandHandler<Command>> _mockHandler;
        private Mock<IAsyncCommandHandler<Command>> _mockAsyncHandler;
        private CancellationToken _cancellationToken;
        private CommandDispatcher _dispatcher;

        [SetUp]
        public void SetUp()
        {
            _mockResolver = new Mock<ICommandHandlerResolver>();
            _mockHandler = new Mock<ICommandHandler<Command>>();
            _mockAsyncHandler = new Mock<IAsyncCommandHandler<Command>>();
            _cancellationToken = new CancellationTokenSource().Token;
            _dispatcher = new CommandDispatcher(_mockResolver.Object);
        }

        [Test]
        public void Dispatch_Throws_An_Exception_For_Null_Command()
        {
            Assert.Throws<ArgumentNullException>(
                () => _dispatcher.Dispatch(null));
        }

        [Test]
        public void Dispatch_Calls_Sync_Handler_If_It_Can_Be_Resolved()
        {
            var command = new Command();
            _mockHandler
                .Setup(x => x.Handle(command));
            _mockResolver
                .SetupResolveSyncHandler(_mockHandler.Object)
                .Returns(true);

            _dispatcher.Dispatch(command);

            _mockHandler.VerifyAll();
        }

        [Test]
        public void Dispatch_Calls_Async_Handler_If_Sync_One_CanNot_Be_Resolved()
        {
            var command = new Command();
            _mockAsyncHandler
                .Setup(x => x.HandleAsync(command, CancellationToken.None))
                .Returns(Task.CompletedTask);
            _mockResolver
                .SetupResolveSyncHandler(_mockHandler.Object)
                .Returns(false);
            _mockResolver
                .SetupResolveAsyncHandler(_mockAsyncHandler.Object)
                .Returns(true);

            _dispatcher.Dispatch(command);

            _mockAsyncHandler.VerifyAll();
        }

        [Test]
        public void Dispatch_Throws_An_Exception_If_Sync_Handler_Throws()
        {
            var command = new Command();
            var exception = new Exception();

            _mockHandler
                .Setup(x => x.Handle(command))
                .Throws(exception);
            _mockResolver
                .SetupResolveSyncHandler(_mockHandler.Object)
                .Returns(true);

            var result =
                Assert.Throws<Exception>(() => _dispatcher.Dispatch(command));

            Assert.That(result, Is.SameAs(exception));
        }

        [Test]
        public void Dispatch_Throws_An_Exception_If_Async_Handler_Throws()
        {
            var command = new Command();
            var exception = new Exception();

            _mockAsyncHandler
                .Setup(x => x.HandleAsync(command, CancellationToken.None))
                .Throws(exception);
            _mockResolver
                .SetupResolveSyncHandler(_mockHandler.Object)
                .Returns(false);
            _mockResolver
               .SetupResolveAsyncHandler(_mockAsyncHandler.Object)
               .Returns(true);

            var result =
                Assert.Throws<Exception>(() => _dispatcher.Dispatch(command));

            Assert.That(result, Is.SameAs(exception));
        }

        [Test]
        public void Dispatch_Throws_An_Exception_If_Async_Handler_Throws_Aggregate_One()
        {
            var command = new Command();
            var exception = new Exception();

            _mockAsyncHandler
                .Setup(x => x.HandleAsync(command, CancellationToken.None))
                .Throws(new AggregateException(exception));
            _mockResolver
                .SetupResolveSyncHandler(_mockHandler.Object)
                .Returns(false);
            _mockResolver
               .SetupResolveAsyncHandler(_mockAsyncHandler.Object)
               .Returns(true);

            var result =
                Assert.Throws<Exception>(() => _dispatcher.Dispatch(command));

            Assert.That(result, Is.SameAs(exception));
        }

        [Test]
        public void Dispatch_Throws_An_Exception_If_Both_Sync_And_Async_Handlers_CanNot_Be_Resolved()
        {
            var command = new Command();

            _mockResolver
                .SetupResolveSyncHandler(_mockHandler.Object)
                .Returns(false);
            _mockResolver
               .SetupResolveAsyncHandler(_mockAsyncHandler.Object)
               .Returns(false);

            Assert.Throws<HandlerNotFoundException>(
                    () => _dispatcher.Dispatch(command)
                );
        }

        [Test]
        public void DispatchAsync_Throws_An_Exception_For_Null_Command()
        {
            Assert.ThrowsAsync<ArgumentNullException>(
                () => _dispatcher.DispatchAsync(null, _cancellationToken));
        }

        [Test]
        public async Task DispatchAsync_Calls_Async_Handler_If_It_Can_Be_Resolved()
        {
            var command = new Command();
            _mockAsyncHandler
                .Setup(x => x.HandleAsync(command, _cancellationToken))
                .Returns(Task.CompletedTask);
            _mockResolver
                .SetupResolveAsyncHandler(_mockAsyncHandler.Object)
                .Returns(true);

            await _dispatcher.DispatchAsync(command, _cancellationToken);

            _mockAsyncHandler.VerifyAll();
        }

        [Test]
        public async Task DispatchAsync_Calls_Sync_Handler_If_Async_One_CanNot_Be_Resolved()
        {
            var command = new Command();
            _mockHandler
                .Setup(x => x.Handle(command));
            _mockResolver
                .SetupResolveAsyncHandler(_mockAsyncHandler.Object)
                .Returns(false);
            _mockResolver
                .SetupResolveSyncHandler(_mockHandler.Object)
                .Returns(true);

            await _dispatcher.DispatchAsync(command, _cancellationToken);

            _mockHandler.VerifyAll();
        }

        [Test]
        public void DispatchAsync_Throws_An_Exception_If_Async_Handler_Throws()
        {
            var command = new Command();
            var exception = new Exception();

            _mockAsyncHandler
                .Setup(x => x.HandleAsync(command, _cancellationToken))
                .Throws(exception);
            _mockResolver
                .SetupResolveAsyncHandler(_mockAsyncHandler.Object)
                .Returns(true);

            var result =
                Assert.ThrowsAsync<Exception>(() => _dispatcher.DispatchAsync(command, _cancellationToken));

            Assert.That(result, Is.SameAs(exception));
        }

        [Test]
        public void DispatchAsync_Throws_An_Exception_If_Sync_Handler_Throws()
        {
            var command = new Command();
            var exception = new Exception();

            _mockHandler
                .Setup(x => x.Handle(command))
                .Throws(exception);
            _mockResolver
                .SetupResolveSyncHandler(_mockHandler.Object)
                .Returns(true);
            _mockResolver
               .SetupResolveAsyncHandler(_mockAsyncHandler.Object)
               .Returns(false);

            var result =
                Assert.ThrowsAsync<Exception>(() => _dispatcher.DispatchAsync(command, _cancellationToken));

            Assert.That(result, Is.SameAs(exception));
        }

        [Test]
        public void DispatchAsync_Throws_An_Exception_If_Both_Sync_And_Async_Handlers_CanNot_Be_Resolved()
        {
            var command = new Command();

            _mockResolver
                .SetupResolveSyncHandler(_mockHandler.Object)
                .Returns(false);
            _mockResolver
               .SetupResolveAsyncHandler(_mockAsyncHandler.Object)
               .Returns(false);

            Assert.ThrowsAsync<HandlerNotFoundException>(
                    () => _dispatcher.DispatchAsync(command, _cancellationToken));
        }

        public class Command : ICommand
        {
        }
    }

    public static class CommandHandlerResolverMock
    {
        public static ISetup<ICommandHandlerResolver, bool> SetupResolveSyncHandler<TCommand>(
            this Mock<ICommandHandlerResolver> resolver, ICommandHandler<TCommand> handler = null)
            where TCommand : ICommand
        {
            object h = handler;
            return resolver
                .Setup(x => x.TryResolveHandler(typeof(ICommandHandler<TCommand>), out h));
        }

        public static ISetup<ICommandHandlerResolver, bool> SetupResolveAsyncHandler<TCommand>(
            this Mock<ICommandHandlerResolver> resolver, IAsyncCommandHandler<TCommand> handler = null)
            where TCommand : ICommand
        {
            object h = handler;
            return resolver
                .Setup(x => x.TryResolveHandler(typeof(IAsyncCommandHandler<TCommand>), out h));
        }
    }
}
