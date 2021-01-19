using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Quarks.CQRS.Impl;

namespace Quarks.CQRS.Tests
{
    [TestFixture]
    public class ServiceCollectionTests
    {
        [Test]
        public void Can_Be_Used_With_ServiceCollection()
        {
            var services = new ServiceCollection()
                .AddTransient<ICommandDispatcher, CommandDispatcher>()
                .AddTransient<IQueryDispatcher, QueryDispatcher>()
                .BuildServiceProvider();

            var commandDispatcher = services.GetRequiredService<ICommandDispatcher>();
            Assert.That(commandDispatcher, Is.Not.Null);

            var queryDispatcher = services.GetRequiredService<IQueryDispatcher>();
            Assert.That(queryDispatcher, Is.Not.Null);
        }
    }
}