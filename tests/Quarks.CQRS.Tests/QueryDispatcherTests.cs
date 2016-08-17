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
		private Mock<IQueryHandlerFactory> _mockHandlerFactory;
		private QueryDispatcher _dispatcher;

		[SetUp]
		public void SetUp()
		{
			_cancellationToken = CancellationToken.None;
			_mockHandlerFactory = new Mock<IQueryHandlerFactory>();
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

		public class FakeQuery : IQuery<FakeModel>
		{
			
		}

		public class FakeModel
		{
			
		}
	}
}
