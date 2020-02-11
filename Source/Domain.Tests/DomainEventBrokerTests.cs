// ReSharper disable ObjectCreationAsStatement
// ReSharper disable PossibleNullReferenceException

using System;
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
        private IAsyncDomainEventHandler<TestDomainEvent> asyncDomainEventHandler1;
        private IAsyncDomainEventHandler<TestDomainEvent> asyncDomainEventHandler2;
        private IAsyncDomainEventHandler<AnotherTestDomainEvent> asyncDomainEventHandler3;

        [TestInitialize]
        public void Setup()
        {
            domainEventHandler1 = Substitute.For<IDomainEventHandler<TestDomainEvent>>();
            domainEventHandler2 = Substitute.For<IDomainEventHandler<TestDomainEvent>>();
            domainEventHandler3 = Substitute.For<IDomainEventHandler<AnotherTestDomainEvent>>();
            asyncDomainEventHandler1 = Substitute.For<IAsyncDomainEventHandler<TestDomainEvent>>();
            asyncDomainEventHandler2 = Substitute.For<IAsyncDomainEventHandler<TestDomainEvent>>();
            asyncDomainEventHandler3 = Substitute.For<IAsyncDomainEventHandler<AnotherTestDomainEvent>>();

            resolver = Substitute.For<IDomainEventHandlerResolver>();
            resolver.ResolveEventHandlers<IDomainEventHandler<TestDomainEvent>>().Returns(new[] { domainEventHandler1, domainEventHandler2 });
            resolver.ResolveEventHandlers<IDomainEventHandler<TestDomainEvent>>().Returns(new[] { domainEventHandler1, domainEventHandler2 });
            resolver.ResolveEventHandlers<IDomainEventHandler<AnotherTestDomainEvent>>().Returns(new[] { domainEventHandler3 });
            resolver.ResolveEventHandlers<IAsyncDomainEventHandler<TestDomainEvent>>().Returns(new[] { asyncDomainEventHandler1, asyncDomainEventHandler2 });
            resolver.ResolveEventHandlers<IAsyncDomainEventHandler<TestDomainEvent>>().Returns(new[] { asyncDomainEventHandler1, asyncDomainEventHandler2 });
            resolver.ResolveEventHandlers<IAsyncDomainEventHandler<AnotherTestDomainEvent>>().Returns(new[] { asyncDomainEventHandler3 });

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
        public void EventCannotBeNull()
        {
            sut.PublishEvent(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void EventsListCannotBeNull()
        {
            sut.PublishEvents(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void EventsListCannotContainsNulls()
        {
            sut.PublishEvents(new[] { testEvent1, null });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AsyncEventCannotBeNull()
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
        public void AsyncEventsListCannotBeNull()
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
        public void AsyncEventsListCannotContainsNulls()
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
            sut.PublishEvent(testEvent1);

            Received.InOrder(() =>
            {
                domainEventHandler1.Received().Execute(testEvent1);
                domainEventHandler2.Received().Execute(testEvent1);
                asyncDomainEventHandler1.Received().ExecuteAsync(testEvent1);
                asyncDomainEventHandler2.Received().ExecuteAsync(testEvent1);
            });

            domainEventHandler3.DidNotReceive().Execute(Arg.Any<AnotherTestDomainEvent>());
            asyncDomainEventHandler3.DidNotReceive().ExecuteAsync(Arg.Any<AnotherTestDomainEvent>());
        }

        [TestMethod]
        public void EventHandlersAreExecutedWithMultipleEvents()
        {
            sut.PublishEvents(new[] { testEvent2, testEvent1 });

            Received.InOrder(() =>
            {
                domainEventHandler1.Received().Execute(testEvent2);
                domainEventHandler2.Received().Execute(testEvent2);
                asyncDomainEventHandler1.Received().ExecuteAsync(testEvent2);
                asyncDomainEventHandler2.Received().ExecuteAsync(testEvent2);
                domainEventHandler1.Received().Execute(testEvent1);
                domainEventHandler2.Received().Execute(testEvent1);
                asyncDomainEventHandler1.Received().ExecuteAsync(testEvent1);
                asyncDomainEventHandler2.Received().ExecuteAsync(testEvent1);
            });

            domainEventHandler3.DidNotReceive().Execute(Arg.Any<AnotherTestDomainEvent>());
            asyncDomainEventHandler3.DidNotReceive().ExecuteAsync(Arg.Any<AnotherTestDomainEvent>());
        }

        [TestMethod]
        public void AsyncEventHandlersAreExecutedWithSingleEvent()
        {
            sut.PublishEventAsync(testEvent1).Wait();

            Received.InOrder(() =>
            {
                domainEventHandler1.Received().Execute(testEvent1);
                domainEventHandler2.Received().Execute(testEvent1);
                asyncDomainEventHandler1.Received().ExecuteAsync(testEvent1);
                asyncDomainEventHandler2.Received().ExecuteAsync(testEvent1);
            });

            domainEventHandler3.DidNotReceive().Execute(Arg.Any<AnotherTestDomainEvent>());
            asyncDomainEventHandler3.DidNotReceive().ExecuteAsync(Arg.Any<AnotherTestDomainEvent>());
        }

        [TestMethod]
        public void AsyncEventHandlersAreExecutedWithMultipleEvents()
        {
            sut.PublishEventsAsync(new[] { testEvent2, testEvent1 }).Wait();

            Received.InOrder(() =>
            {
                domainEventHandler1.Received().Execute(testEvent2);
                domainEventHandler2.Received().Execute(testEvent2);
                asyncDomainEventHandler1.Received().ExecuteAsync(testEvent2);
                asyncDomainEventHandler2.Received().ExecuteAsync(testEvent2);
                domainEventHandler1.Received().Execute(testEvent1);
                domainEventHandler2.Received().Execute(testEvent1);
                asyncDomainEventHandler1.Received().ExecuteAsync(testEvent1);
                asyncDomainEventHandler2.Received().ExecuteAsync(testEvent1);
            });

            domainEventHandler3.DidNotReceive().Execute(Arg.Any<AnotherTestDomainEvent>());
            asyncDomainEventHandler3.DidNotReceive().ExecuteAsync(Arg.Any<AnotherTestDomainEvent>());
        }
    }
}