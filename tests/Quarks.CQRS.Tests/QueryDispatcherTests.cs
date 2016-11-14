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
    public class QueryDispatcherTests
    {
        private Mock<IQueryHandlerResolver> _mockResolver;
        private Mock<IQueryHandler<Query, int>> _mockHandler;
        private Mock<IAsyncQueryHandler<Query, int>> _mockAsyncHandler;
        private CancellationToken _cancellationToken;
        private QueryDispatcher _dispatcher;

        [SetUp]
        public void SetUp()
        {
            _mockResolver = new Mock<IQueryHandlerResolver>();
            _mockHandler = new Mock<IQueryHandler<Query, int>>();
            _mockAsyncHandler = new Mock<IAsyncQueryHandler<Query, int>>();
            _cancellationToken = new CancellationTokenSource().Token;
            _dispatcher = new QueryDispatcher(_mockResolver.Object);
        }

        [Test]
        public void Dispatch_Throws_An_Exception_For_Null_Query()
        {
            Assert.Throws<ArgumentNullException>(
                () => _dispatcher.Dispatch<int>(null));
        }

        [Test]
        public void Dispatch_Calls_Sync_Handler_If_It_Can_Be_Resolved()
        {
            const int expected = 10;

            var query = new Query();
            _mockHandler
                .Setup(x => x.Handle(query))
                .Returns(expected);
            _mockResolver
                .SetupResolveSyncHandler(_mockHandler.Object)
                .Returns(true);

            int result = _dispatcher.Dispatch<int>(query);

            Assert.That(result, Is.EqualTo(expected));
            _mockHandler.VerifyAll();
        }

        [Test]
        public void Dispatch_Calls_Async_Handler_If_Sync_One_CanNot_Be_Resolved()
        {
            const int expected = 10;

            var query = new Query();
            _mockAsyncHandler
                .Setup(x => x.HandleAsync(query, CancellationToken.None))
                .ReturnsAsync(expected);
            _mockResolver
                .SetupResolveSyncHandler(_mockHandler.Object)
                .Returns(false);
            _mockResolver
                .SetupResolveAsyncHandler(_mockAsyncHandler.Object)
                .Returns(true);

            int result =
                _dispatcher.Dispatch<int>(query);

            Assert.That(result, Is.EqualTo(expected));
            _mockHandler.VerifyAll();
        }

        [Test]
        public void Dispatch_Throws_An_Exception_If_Sync_Handler_Throws()
        {
            var query = new Query();
            var exception = new Exception();

            _mockHandler
                .Setup(x => x.Handle(query))
                .Throws(exception);
            _mockResolver
                .SetupResolveSyncHandler(_mockHandler.Object)
                .Returns(true);

            Exception result =
                Assert.Throws<Exception>(
                    () => _dispatcher.Dispatch<int>(query));

            Assert.That(result, Is.SameAs(exception));
        }

        [Test]
        public void Dispatch_Throws_An_Exception_If_Async_Handler_Throws()
        {
            var query = new Query();
            var exception = new Exception();

            _mockAsyncHandler
                .Setup(x => x.HandleAsync(query, CancellationToken.None))
                .Throws(exception);
            _mockResolver
                .SetupResolveSyncHandler(_mockHandler.Object)
                .Returns(false);
            _mockResolver
               .SetupResolveAsyncHandler(_mockAsyncHandler.Object)
               .Returns(true);

            Exception result =
                Assert.Throws<Exception>(
                    () => _dispatcher.Dispatch<int>(query));

            Assert.That(result, Is.SameAs(exception));
        }

        [Test]
        public void Dispatch_Throws_An_Exception_If_Async_Handler_Throws_Aggregate_One()
        {
            var query = new Query();
            var exception = new Exception();

            _mockAsyncHandler
                .Setup(x => x.HandleAsync(query, CancellationToken.None))
                .Throws(new AggregateException(exception));
            _mockResolver
                .SetupResolveSyncHandler(_mockHandler.Object)
                .Returns(false);
            _mockResolver
               .SetupResolveAsyncHandler(_mockAsyncHandler.Object)
               .Returns(true);

            Exception result =
                Assert.Throws<Exception>(
                    () => _dispatcher.Dispatch<int>(query));

            Assert.That(result, Is.SameAs(exception));
        }

        [Test]
        public void Dispatch_Throws_An_Exception_If_Both_Sync_And_Async_Handlers_CanNot_Be_Resolved()
        {
            var query = new Query();

            _mockResolver
                .SetupResolveSyncHandler(_mockHandler.Object)
                .Returns(false);
            _mockResolver
               .SetupResolveAsyncHandler(_mockAsyncHandler.Object)
               .Returns(false);

            Assert.Throws<HandlerNotFoundException>(
                    () => _dispatcher.Dispatch<int>(query));
        }

        [Test]
        public void DispatchAsync_Throws_An_Exception_For_Null_Query()
        {
            Assert.ThrowsAsync<ArgumentNullException>(
                () => _dispatcher.DispatchAsync<int>(null, _cancellationToken));
        }

        [Test]
        public async Task DispatchAsync_Calls_Async_Handler_If_It_Can_Be_Resolved()
        {
            const int expected = 10;

            var query = new Query();
            _mockAsyncHandler
                .Setup(x => x.HandleAsync(query, _cancellationToken))
                .ReturnsAsync(expected);
            _mockResolver
                .SetupResolveAsyncHandler(_mockAsyncHandler.Object)
                .Returns(true);

            int result =
                await _dispatcher.DispatchAsync<int>(query, _cancellationToken);

            Assert.That(result, Is.EqualTo(expected));
            _mockAsyncHandler.VerifyAll();
        }

        [Test]
        public async Task DispatchAsync_Calls_Sync_Handler_If_Async_One_CanNot_Be_Resolved()
        {
            const int expected = 10;

            var query = new Query();
            _mockHandler
                .Setup(x => x.Handle(query))
                .Returns(expected);
            _mockResolver
                .SetupResolveAsyncHandler(_mockAsyncHandler.Object)
                .Returns(false);
            _mockResolver
                .SetupResolveSyncHandler(_mockHandler.Object)
                .Returns(true);

            int result =
                await _dispatcher.DispatchAsync<int>(query, _cancellationToken);

            Assert.That(result, Is.EqualTo(expected));
            _mockHandler.VerifyAll();
        }

        [Test]
        public void DispatchAsync_Throws_An_Exception_If_Async_Handler_Throws()
        {
            var query = new Query();
            var exception = new Exception();

            _mockAsyncHandler
                .Setup(x => x.HandleAsync(query, _cancellationToken))
                .Throws(exception);
            _mockResolver
                .SetupResolveAsyncHandler(_mockAsyncHandler.Object)
                .Returns(true);

            Exception result =
                Assert.ThrowsAsync<Exception>(
                    () => _dispatcher.DispatchAsync<int>(query, _cancellationToken));

            Assert.That(result, Is.SameAs(exception));
        }

        [Test]
        public void DispatchAsync_Throws_An_Exception_If_Sync_Handler_Throws()
        {
            var query = new Query();
            var exception = new Exception();

            _mockHandler
                .Setup(x => x.Handle(query))
                .Throws(exception);
            _mockResolver
               .SetupResolveAsyncHandler(_mockAsyncHandler.Object)
               .Returns(false);
            _mockResolver
                .SetupResolveSyncHandler(_mockHandler.Object)
                .Returns(true);

            Exception result =
                Assert.ThrowsAsync<Exception>(
                    () => _dispatcher.DispatchAsync<int>(query, _cancellationToken));

            Assert.That(result, Is.SameAs(exception));
        }

        [Test]
        public void DispatchAsync_Throws_An_Exception_If_Both_Sync_And_Async_Handlers_CanNot_Be_Resolved()
        {
            var query = new Query();

            _mockResolver
                .SetupResolveSyncHandler(_mockHandler.Object)
                .Returns(false);
            _mockResolver
               .SetupResolveAsyncHandler(_mockAsyncHandler.Object)
               .Returns(false);

            Assert.ThrowsAsync<HandlerNotFoundException>(
                    () => _dispatcher.DispatchAsync<int>(query, _cancellationToken));
        }

        public class Query : IQuery
        {
        }
    }

    public static class QueryHandlerResolverMock
    {
        public static ISetup<IQueryHandlerResolver, bool> SetupResolveSyncHandler<TQuery, TResult>(
            this Mock<IQueryHandlerResolver> resolver, IQueryHandler<TQuery, TResult> handler = null)
            where TQuery : IQuery
        {
            object h = handler;
            return resolver
                .Setup(x => x.TryResolveHandler(typeof(IQueryHandler<TQuery, TResult>), out h));
        }

        public static ISetup<IQueryHandlerResolver, bool> SetupResolveAsyncHandler<TQuery, TResult>(
           this Mock<IQueryHandlerResolver> resolver, IAsyncQueryHandler<TQuery, TResult> handler = null)
           where TQuery : IQuery
        {
            object h = handler;
            return resolver
                .Setup(x => x.TryResolveHandler(typeof(IAsyncQueryHandler<TQuery, TResult>), out h));
        }
    }
}