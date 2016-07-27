using System;
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
		private Mock<IServiceProvider> _mockServiceProvider;
		private QueryDispatcher _dispatcher;

		[SetUp]
		public void SetUp()
		{
			_cancellationToken = CancellationToken.None;
			_mockServiceProvider = new Mock<IServiceProvider>();
			_dispatcher = new QueryDispatcher(_mockServiceProvider.Object);
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

			_mockServiceProvider
				.Setup(x => x.GetService(typeof(IQueryHandler<FakeQuery, FakeModel>)))
				.Returns(handler.Object);

			FakeModel result =
				await _dispatcher.DispatchAsync<FakeQuery, FakeModel>(fakeQuery, _cancellationToken);

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

			_mockServiceProvider
				.Setup(x => x.GetService(typeof(IQueryHandler<FakeQuery, FakeModel>)))
				.Returns(handler.Object);

			FakeModel result =
				await _dispatcher.DispatchAsync<FakeQuery, FakeModel>(fakeQuery);

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

			_mockServiceProvider
				.Setup(x => x.GetService(typeof(IQueryHandler<FakeQuery, FakeModel>)))
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

			_mockServiceProvider
				.Setup(x => x.GetService(typeof(IQueryHandler<FakeQuery, FakeModel>)))
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
