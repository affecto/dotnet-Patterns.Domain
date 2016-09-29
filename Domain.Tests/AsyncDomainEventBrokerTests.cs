// ReSharper disable ObjectCreationAsStatement

using System;
using Affecto.Patterns.Domain.Tests.TestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Affecto.Patterns.Domain.Tests
{
    [TestClass]
    public class AsyncDomainEventBrokerTests
    {
        private DomainEventBroker sut;
        private IDomainEventHandlerResolver resolver;

        private readonly TestDomainEvent testEvent1 = new TestDomainEvent(Guid.NewGuid());
        private readonly TestDomainEvent testEvent2 = new TestDomainEvent(Guid.NewGuid());
        
        private IAsyncDomainEventHandler<TestDomainEvent> domainEventHandler1;
        private IAsyncDomainEventHandler<TestDomainEvent> domainEventHandler2;
        private IAsyncDomainEventHandler<AnotherTestDomainEvent> domainEventHandler3;

        [TestInitialize]
        public void Setup()
        {
            domainEventHandler1 = Substitute.For<IAsyncDomainEventHandler<TestDomainEvent>>();
            domainEventHandler2 = Substitute.For<IAsyncDomainEventHandler<TestDomainEvent>>();
            domainEventHandler3 = Substitute.For<IAsyncDomainEventHandler<AnotherTestDomainEvent>>();

            resolver = Substitute.For<IDomainEventHandlerResolver>();
            resolver.ResolveEventHandlers<IAsyncDomainEventHandler<TestDomainEvent>>().Returns(new [] { domainEventHandler1, domainEventHandler2 });
            resolver.ResolveEventHandlers<IAsyncDomainEventHandler<TestDomainEvent>>().Returns(new[] { domainEventHandler1, domainEventHandler2 });
            resolver.ResolveEventHandlers<IAsyncDomainEventHandler<AnotherTestDomainEvent>>().Returns(new [] { domainEventHandler3 });

            sut = new DomainEventBroker(resolver);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void EventCannotBeNull()
        {
            try
            {
                sut.PublishEventAsync(null).Wait();
            }
            catch (AggregateException e)
            {
                throw e.InnerException;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void EventsListCannotBeNull()
        {
            try
            {
                sut.PublishEventsAsync(null).Wait();
            }
            catch (AggregateException e)
            {
                throw e.InnerException;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void EventsListCannotContainsNulls()
        {
            try
            {
                sut.PublishEventsAsync(new[] { testEvent1, null }).Wait();
            }
            catch (AggregateException e)
            {
                throw e.InnerException;
            }
        }

        [TestMethod]
        public void EventHandlersAreExecutedWithSingleEvent()
        {
            sut.PublishEventAsync(testEvent1).Wait();

            Received.InOrder(() =>
            {
                domainEventHandler1.Received().ExecuteAsync(testEvent1);
                domainEventHandler2.Received().ExecuteAsync(testEvent1);
            });

            domainEventHandler3.DidNotReceive().ExecuteAsync(Arg.Any<AnotherTestDomainEvent>());
        }

        [TestMethod]
        public void EventHandlersAreExecutedWithMultipleEvents()
        {
            sut.PublishEventsAsync(new [] { testEvent2, testEvent1 }).Wait();

            Received.InOrder(() =>
            {
                domainEventHandler1.Received().ExecuteAsync(testEvent2);
                domainEventHandler2.Received().ExecuteAsync(testEvent2);
                domainEventHandler1.Received().ExecuteAsync(testEvent1);
                domainEventHandler2.Received().ExecuteAsync(testEvent1);
            });

            domainEventHandler3.DidNotReceive().ExecuteAsync(Arg.Any<AnotherTestDomainEvent>());
        }
    }
}