// ReSharper disable ObjectCreationAsStatement
// ReSharper disable PossibleNullReferenceException

using System;
using System.Threading.Tasks;
using Affecto.Patterns.Domain.Tests.TestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Affecto.Patterns.Domain.Tests
{
    [TestClass]
    public class DomainEventBrokerTests
    {
        private DomainEventBroker sut;
        private IDomainEventHandlerResolver resolver;

        private readonly TestDomainEvent testEvent1 = new TestDomainEvent(Guid.NewGuid());
        private readonly TestDomainEvent testEvent2 = new TestDomainEvent(Guid.NewGuid());

        private IDomainEventHandler<TestDomainEvent> domainEventHandler1;
        private IDomainEventHandler<TestDomainEvent> domainEventHandler2;
        private IDomainEventHandler<AnotherTestDomainEvent> domainEventHandler3;

        [TestInitialize]
        public void Setup()
        {
            domainEventHandler1 = Substitute.For<IDomainEventHandler<TestDomainEvent>>();
            domainEventHandler2 = Substitute.For<IDomainEventHandler<TestDomainEvent>>();
            domainEventHandler3 = Substitute.For<IDomainEventHandler<AnotherTestDomainEvent>>();

            resolver = Substitute.For<IDomainEventHandlerResolver>();
            resolver.ResolveEventHandlers<IDomainEventHandler<TestDomainEvent>>().Returns(new[] { domainEventHandler1, domainEventHandler2 });
            resolver.ResolveEventHandlers<IDomainEventHandler<TestDomainEvent>>().Returns(new[] { domainEventHandler1, domainEventHandler2 });
            resolver.ResolveEventHandlers<IDomainEventHandler<AnotherTestDomainEvent>>().Returns(new[] { domainEventHandler3 });

            sut = new DomainEventBroker(resolver);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void EventHandlerResolverCannotBeNull()
        {
            new DomainEventBroker(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task AsyncEventCannotBeNull()
        {

            await sut.PublishEventAsync(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task AsyncEventsListCannotBeNull()
        {
            await sut.PublishEventsAsync(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task AsyncEventsListCannotContainsNulls()
        {
            await sut.PublishEventsAsync(new[] { testEvent1, null });
        }

        [TestMethod]
        public async Task AsyncEventHandlersAreExecutedWithSingleEvent()
        {
            await sut.PublishEventAsync(testEvent1);

            Received.InOrder(() =>
            {
                domainEventHandler1.Received().ExecuteAsync(testEvent1);
                domainEventHandler2.Received().ExecuteAsync(testEvent1);
            });

            await domainEventHandler3.DidNotReceive().ExecuteAsync(Arg.Any<AnotherTestDomainEvent>());
        }

        [TestMethod]
        public async Task AsyncEventHandlersAreExecutedWithMultipleEvents()
        {
            await sut.PublishEventsAsync(new[] { testEvent2, testEvent1 });

            Received.InOrder(() =>
            {
                domainEventHandler1.Received().ExecuteAsync(testEvent2);
                domainEventHandler2.Received().ExecuteAsync(testEvent2);
                domainEventHandler1.Received().ExecuteAsync(testEvent1);
                domainEventHandler2.Received().ExecuteAsync(testEvent1);
            });

            await domainEventHandler3.DidNotReceive().ExecuteAsync(Arg.Any<AnotherTestDomainEvent>());
        }
    }
}