using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Quarks.CQRS.Impl;

namespace Quarks.CQRS.Tests
{
	[TestFixture]
    public class QueryDispatcherTests
	{
		private CancellationToken _cancellationToken;
		private Mock<IHandlerFactory> _mockHandlerFactory;
		private QueryDispatcher _dispatcher;

		[SetUp]
		public void SetUp()
		{
			_cancellationToken = CancellationToken.None;
			_mockHandlerFactory = new Mock<IHandlerFactory>();
			_dispatcher = new QueryDispatcher(_mockHandlerFactory.Object);
		}

		[Test]
		public async Task Dispatch_Resolves_Handler_And_Calls_It()
		{
			var fakeQuery = new FakeQuery();
			var fakeModel = new FakeModel();

			var handler = new Mock<IQueryHandler<FakeQuery, FakeModel>>();
			handler
				.Setup(x => x.HandleAsync(fakeQuery, _cancellationToken))
				.ReturnsAsync(fakeModel);

			_mockHandlerFactory
				.Setup(x => x.CreateHandler(typeof(IQueryHandler<FakeQuery, FakeModel>)))
				.Returns(handler.Object);

			FakeModel result =
				await _dispatcher.DispatchAsync(fakeQuery, _cancellationToken);

			Assert.That(result, Is.EqualTo(fakeModel));
		}

		[Test]
		public async Task Dispatch_With_No_CancellationToken_Resolves_Handler_And_Calls_It()
		{
			var fakeQuery = new FakeQuery();
			var fakeModel = new FakeModel();

			var handler = new Mock<IQueryHandler<FakeQuery, FakeModel>>();
			handler
				.Setup(x => x.HandleAsync(fakeQuery, CancellationToken.None))
				.ReturnsAsync(fakeModel);

			_mockHandlerFactory
				.Setup(x => x.CreateHandler(typeof(IQueryHandler<FakeQuery, FakeModel>)))
				.Returns(handler.Object);

			FakeModel result =
				await _dispatcher.DispatchAsync(fakeQuery);

			Assert.That(result, Is.EqualTo(fakeModel));
		}

		[Test]
		public async Task For()
		{
			var fakeQuery = new FakeQuery();
			var fakeModel = new FakeModel();

			var handler = new Mock<IQueryHandler<FakeQuery, FakeModel>>();
			handler
				.Setup(x => x.HandleAsync(fakeQuery, _cancellationToken))
				.ReturnsAsync(fakeModel);

			_mockHandlerFactory
				.Setup(x => x.CreateHandler(typeof(IQueryHandler<FakeQuery, FakeModel>)))
				.Returns(handler.Object);

			FakeModel result =
				await _dispatcher
					.For<FakeModel>()
					.DispatchAsync(fakeQuery, _cancellationToken);

			Assert.That(result, Is.EqualTo(fakeModel));
		}

		[Test]
		public async Task For_With_No_CancellationToken()
		{
			var fakeQuery = new FakeQuery();
			var fakeModel = new FakeModel();

			var handler = new Mock<IQueryHandler<FakeQuery, FakeModel>>();
			handler
				.Setup(x => x.HandleAsync(fakeQuery, CancellationToken.None))
				.ReturnsAsync(fakeModel);

			_mockHandlerFactory
				.Setup(x => x.CreateHandler(typeof(IQueryHandler<FakeQuery, FakeModel>)))
				.Returns(handler.Object);

			FakeModel result =
				await _dispatcher
					.For<FakeModel>()
					.DispatchAsync(fakeQuery);

			Assert.That(result, Is.EqualTo(fakeModel));
		}

		public class FakeQuery : IQuery<FakeModel>
		{
			
		}

		public class FakeModel
		{
			
		}
	}
}
