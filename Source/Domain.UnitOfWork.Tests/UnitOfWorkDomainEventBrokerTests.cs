// ReSharper disable ObjectCreationAsStatement
// ReSharper disable PossibleNullReferenceException

using System;
using System.Threading.Tasks;
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

        [TestInitialize]
        public void Setup()
        {
            unitOfWork = new TestUnitOfWork();
            domainEventHandler1 = Substitute.For<IUnitOfWorkDomainEventHandler<TestDomainEvent, TestUnitOfWork>>();
            domainEventHandler2 = Substitute.For<IUnitOfWorkDomainEventHandler<TestDomainEvent, TestUnitOfWork>>();
            domainEventHandler3 = Substitute.For<IUnitOfWorkDomainEventHandler<AnotherTestDomainEvent, TestUnitOfWork>>();

            resolver = Substitute.For<IUnitOfWorkDomainEventHandlerResolver>();
            resolver.ResolveEventHandlers<IUnitOfWorkDomainEventHandler<TestDomainEvent, TestUnitOfWork>>().Returns(new[] { domainEventHandler1, domainEventHandler2 });
            resolver.ResolveEventHandlers<IUnitOfWorkDomainEventHandler<TestDomainEvent, TestUnitOfWork>>().Returns(new[] { domainEventHandler1, domainEventHandler2 });
            resolver.ResolveEventHandlers<IUnitOfWorkDomainEventHandler<AnotherTestDomainEvent, TestUnitOfWork>>().Returns(new[] { domainEventHandler3 });

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
                domainEventHandler1.Received().ExecuteAsync(testEvent1, unitOfWork);
                domainEventHandler2.Received().ExecuteAsync(testEvent1, unitOfWork);
            });

            await domainEventHandler3.DidNotReceive().ExecuteAsync(Arg.Any<AnotherTestDomainEvent>(), Arg.Any<TestUnitOfWork>());
        }

        [TestMethod]
        public async Task AsyncEventHandlersAreExecutedWithMultipleEvents()
        {
            await sut.PublishEventsAsync(new[] { testEvent2, testEvent1 });

            Received.InOrder(() =>
            {
                domainEventHandler1.Received().ExecuteAsync(testEvent2, unitOfWork);
                domainEventHandler2.Received().ExecuteAsync(testEvent2, unitOfWork);
                domainEventHandler1.Received().ExecuteAsync(testEvent1, unitOfWork);
                domainEventHandler2.Received().ExecuteAsync(testEvent1, unitOfWork);
            });

            await domainEventHandler3.DidNotReceive().ExecuteAsync(Arg.Any<AnotherTestDomainEvent>(), Arg.Any<TestUnitOfWork>());
        }
    }
}