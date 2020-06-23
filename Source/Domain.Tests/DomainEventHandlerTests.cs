using System;
using System.Threading.Tasks;
using Affecto.Patterns.Domain.Tests.TestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Affecto.Patterns.Domain.Tests
{
    [TestClass]
    public class DomainEventHandlerTests
    {
        private DomainEventHandler<TestDomainEvent> sut;

        [TestInitialize]
        public void Setup()
        {
            sut = Substitute.For<DomainEventHandler<TestDomainEvent>>();
        }

        [TestMethod]
        public async Task SynchronousMethodIsCalledFromAsynchronousVersion()
        {
            var domainEvent = new TestDomainEvent(Guid.NewGuid());
            await sut.ExecuteAsync(domainEvent);

            sut.Received().Execute(domainEvent);
        }
    }
}