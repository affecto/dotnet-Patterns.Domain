// ReSharper disable ObjectCreationAsStatement
// ReSharper disable PossibleNullReferenceException

using System;
using Affecto.Patterns.Domain.Tests.TestHelpers;
using Affecto.Patterns.Domain.UnitOfWork.Tests.TestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Affecto.Patterns.Domain.UnitOfWork.Tests
{
    [TestClass]
    public class UnitOfWorkDomainEventBrokerTests
    {
        private UnitOfWorkDomainEventBroker<TestUnitOfWork> sut;
        private TestUnitOfWork unitOfWork;
        private IUnitOfWorkDomainEventHandlerResolver resolver;

        private readonly TestDomainEvent testEvent1 = new TestDomainEvent(Guid.NewGuid());
        private readonly TestDomainEvent testEvent2 = new TestDomainEvent(Guid.NewGuid());

        private IUnitOfWorkDomainEventHandler<TestDomainEvent, TestUnitOfWork> domainEventHandler1;
        private IUnitOfWorkDomainEventHandler<TestDomainEvent, TestUnitOfWork> domainEventHandler2;
        private IUnitOfWorkDomainEventHandler<AnotherTestDomainEvent, TestUnitOfWork> domainEventHandler3;
        private IAsyncUnitOfWorkDomainEventHandler<TestDomainEvent, TestUnitOfWork> asyncDomainEventHandler1;
        private IAsyncUnitOfWorkDomainEventHandler<TestDomainEvent, TestUnitOfWork> asyncDomainEventHandler2;
        private IAsyncUnitOfWorkDomainEventHandler<AnotherTestDomainEvent, TestUnitOfWork> asyncDomainEventHandler3;

        [TestInitialize]
        public void Setup()
        {
            unitOfWork = new TestUnitOfWork();
            domainEventHandler1 = Substitute.For<IUnitOfWorkDomainEventHandler<TestDomainEvent, TestUnitOfWork>>();
            domainEventHandler2 = Substitute.For<IUnitOfWorkDomainEventHandler<TestDomainEvent, TestUnitOfWork>>();
            domainEventHandler3 = Substitute.For<IUnitOfWorkDomainEventHandler<AnotherTestDomainEvent, TestUnitOfWork>>();
            asyncDomainEventHandler1 = Substitute.For<IAsyncUnitOfWorkDomainEventHandler<TestDomainEvent, TestUnitOfWork>>();
            asyncDomainEventHandler2 = Substitute.For<IAsyncUnitOfWorkDomainEventHandler<TestDomainEvent, TestUnitOfWork>>();
            asyncDomainEventHandler3 = Substitute.For<IAsyncUnitOfWorkDomainEventHandler<AnotherTestDomainEvent, TestUnitOfWork>>();

            resolver = Substitute.For<IUnitOfWorkDomainEventHandlerResolver>();
            resolver.ResolveEventHandlers<IUnitOfWorkDomainEventHandler<TestDomainEvent, TestUnitOfWork>>().Returns(new[] { domainEventHandler1, domainEventHandler2 });
            resolver.ResolveEventHandlers<IUnitOfWorkDomainEventHandler<TestDomainEvent, TestUnitOfWork>>().Returns(new[] { domainEventHandler1, domainEventHandler2 });
            resolver.ResolveEventHandlers<IUnitOfWorkDomainEventHandler<AnotherTestDomainEvent, TestUnitOfWork>>().Returns(new[] { domainEventHandler3 });
            resolver.ResolveEventHandlers<IAsyncUnitOfWorkDomainEventHandler<TestDomainEvent, TestUnitOfWork>>().Returns(new[] { asyncDomainEventHandler1, asyncDomainEventHandler2 });
            resolver.ResolveEventHandlers<IAsyncUnitOfWorkDomainEventHandler<TestDomainEvent, TestUnitOfWork>>().Returns(new[] { asyncDomainEventHandler1, asyncDomainEventHandler2 });
            resolver.ResolveEventHandlers<IAsyncUnitOfWorkDomainEventHandler<AnotherTestDomainEvent, TestUnitOfWork>>().Returns(new[] { asyncDomainEventHandler3 });

            sut = new UnitOfWorkDomainEventBroker<TestUnitOfWork>(resolver, unitOfWork);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void EventHandlerResolverCannotBeNull()
        {
            new UnitOfWorkDomainEventBroker<TestUnitOfWork>(null, unitOfWork);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void UnitOfWorkCannotBeNull()
        {
            new UnitOfWorkDomainEventBroker<TestUnitOfWork>(resolver, null);
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
                domainEventHandler1.Received().Execute(testEvent1, unitOfWork);
                domainEventHandler2.Received().Execute(testEvent1, unitOfWork);
                asyncDomainEventHandler1.Received().ExecuteAsync(testEvent1, unitOfWork);
                asyncDomainEventHandler2.Received().ExecuteAsync(testEvent1, unitOfWork);
            });

            domainEventHandler3.DidNotReceive().Execute(Arg.Any<AnotherTestDomainEvent>(), Arg.Any<TestUnitOfWork>());
            asyncDomainEventHandler3.DidNotReceive().ExecuteAsync(Arg.Any<AnotherTestDomainEvent>(), Arg.Any<TestUnitOfWork>());
        }

        [TestMethod]
        public void EventHandlersAreExecutedWithMultipleEvents()
        {
            sut.PublishEvents(new[] { testEvent2, testEvent1 });

            Received.InOrder(() =>
            {
                domainEventHandler1.Received().Execute(testEvent2, unitOfWork);
                domainEventHandler2.Received().Execute(testEvent2, unitOfWork);
                asyncDomainEventHandler1.Received().ExecuteAsync(testEvent2, unitOfWork);
                asyncDomainEventHandler2.Received().ExecuteAsync(testEvent2, unitOfWork);
                domainEventHandler1.Received().Execute(testEvent1, unitOfWork);
                domainEventHandler2.Received().Execute(testEvent1, unitOfWork);
                asyncDomainEventHandler1.Received().ExecuteAsync(testEvent1, unitOfWork);
                asyncDomainEventHandler2.Received().ExecuteAsync(testEvent1, unitOfWork);
            });

            domainEventHandler3.DidNotReceive().Execute(Arg.Any<AnotherTestDomainEvent>(), Arg.Any<TestUnitOfWork>());
            asyncDomainEventHandler3.DidNotReceive().ExecuteAsync(Arg.Any<AnotherTestDomainEvent>(), Arg.Any<TestUnitOfWork>());
        }

        [TestMethod]
        public void AsyncEventHandlersAreExecutedWithSingleEvent()
        {
            sut.PublishEventAsync(testEvent1).Wait();

            Received.InOrder(() =>
            {
                domainEventHandler1.Received().Execute(testEvent1, unitOfWork);
                domainEventHandler2.Received().Execute(testEvent1, unitOfWork);
                asyncDomainEventHandler1.Received().ExecuteAsync(testEvent1, unitOfWork);
                asyncDomainEventHandler2.Received().ExecuteAsync(testEvent1, unitOfWork);
            });

            domainEventHandler3.DidNotReceive().Execute(Arg.Any<AnotherTestDomainEvent>(), Arg.Any<TestUnitOfWork>());
            asyncDomainEventHandler3.DidNotReceive().ExecuteAsync(Arg.Any<AnotherTestDomainEvent>(), Arg.Any<TestUnitOfWork>());
        }

        [TestMethod]
        public void AsyncEventHandlersAreExecutedWithMultipleEvents()
        {
            sut.PublishEventsAsync(new[] { testEvent2, testEvent1 }).Wait();

            Received.InOrder(() =>
            {
                domainEventHandler1.Received().Execute(testEvent2, unitOfWork);
                domainEventHandler2.Received().Execute(testEvent2, unitOfWork);
                asyncDomainEventHandler1.Received().ExecuteAsync(testEvent2, unitOfWork);
                asyncDomainEventHandler2.Received().ExecuteAsync(testEvent2, unitOfWork);
                domainEventHandler1.Received().Execute(testEvent1, unitOfWork);
                domainEventHandler2.Received().Execute(testEvent1, unitOfWork);
                asyncDomainEventHandler1.Received().ExecuteAsync(testEvent1, unitOfWork);
                asyncDomainEventHandler2.Received().ExecuteAsync(testEvent1, unitOfWork);
            });

            domainEventHandler3.DidNotReceive().Execute(Arg.Any<AnotherTestDomainEvent>(), Arg.Any<TestUnitOfWork>());
            asyncDomainEventHandler3.DidNotReceive().ExecuteAsync(Arg.Any<AnotherTestDomainEvent>(), Arg.Any<TestUnitOfWork>());
        }
    }
}