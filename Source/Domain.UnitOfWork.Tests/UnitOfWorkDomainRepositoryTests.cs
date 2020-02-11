using System;
using System.Collections.Generic;
using Affecto.Patterns.Domain.Tests.TestHelpers;
using Affecto.Patterns.Domain.UnitOfWork.Tests.TestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Affecto.Patterns.Domain.UnitOfWork.Tests
{
    [TestClass]
    public class UnitOfWorkDomainRepositoryTests
    {
        private IUnitOfWorkDomainEventHandler<TestDomainEvent, TestUnitOfWork> unitOfWorkDomainEventHandler1;
        private IUnitOfWorkDomainEventHandler<TestDomainEvent, TestUnitOfWork> unitOfWorkDomainEventHandler2;
        private IDomainEventHandler<TestDomainEvent> domainEventHandler3;
        private IDomainEventHandler<TestDomainEvent> domainEventHandler4;

        private IEnumerable<IUnitOfWorkDomainEventHandler<TestDomainEvent, TestUnitOfWork>> unitOfWorkDomainEventHandlers;
        private IEnumerable<IDomainEventHandler<TestDomainEvent>> domainEventHandlers;

        private TestDomainEvent domainEvent;

        private IDomainEventHandlerResolver eventHandlerResolver;
        private IUnitOfWorkDomainEventHandlerResolver unitOfWorkEventHandlerResolver;
        private TestUnitOfWork unitOfWork;

        private TestUnitOfWorkDomainRepository sut;

        [TestInitialize]
        public void Setup()
        {
            unitOfWorkDomainEventHandler1 = Substitute.For<IUnitOfWorkDomainEventHandler<TestDomainEvent, TestUnitOfWork>>();
            unitOfWorkDomainEventHandler2 = Substitute.For<IUnitOfWorkDomainEventHandler<TestDomainEvent, TestUnitOfWork>>();
            
            unitOfWorkDomainEventHandlers = new List<IUnitOfWorkDomainEventHandler<TestDomainEvent, TestUnitOfWork>>
            {
                unitOfWorkDomainEventHandler1,
                unitOfWorkDomainEventHandler2
            };

            domainEventHandler3 = Substitute.For<IDomainEventHandler<TestDomainEvent>>();
            domainEventHandler4 = Substitute.For<IDomainEventHandler<TestDomainEvent>>();
            domainEventHandlers = new List<IDomainEventHandler<TestDomainEvent>>
            {
                domainEventHandler3,
                domainEventHandler4
            };

            eventHandlerResolver = Substitute.For<IDomainEventHandlerResolver>();
            unitOfWorkEventHandlerResolver = Substitute.For<IUnitOfWorkDomainEventHandlerResolver>();
            unitOfWork = Substitute.For<TestUnitOfWork>();

            domainEvent = new TestDomainEvent(Guid.NewGuid());
            unitOfWorkEventHandlerResolver.ResolveEventHandlers<IUnitOfWorkDomainEventHandler<TestDomainEvent, TestUnitOfWork>>().Returns(unitOfWorkDomainEventHandlers);
            eventHandlerResolver.ResolveEventHandlers<IDomainEventHandler<TestDomainEvent>>().Returns(domainEventHandlers);

            sut = new TestUnitOfWorkDomainRepository(eventHandlerResolver, unitOfWorkEventHandlerResolver, unitOfWork);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullUnitOfWorkThrowsException()
        {
            sut = new TestUnitOfWorkDomainRepository(eventHandlerResolver, unitOfWorkEventHandlerResolver, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullAggregateThrowsException()
        {
            sut.ApplyChanges((TestAggregateRoot) null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullAggregatesThrowsException()
        {
            sut.ApplyChanges((IEnumerable<TestAggregateRoot>) null);
        }

        [TestMethod]
        public void ResolvedUnitOfWorkEventHandlersAreExecutedInCorrectOrder()
        {
            TestAggregateRoot aggregateRoot = new TestAggregateRoot(Guid.NewGuid());
            aggregateRoot.ApplyEvent(domainEvent);

            sut.ApplyChanges(aggregateRoot);

            unitOfWorkDomainEventHandler1.Received().Execute(domainEvent, unitOfWork);
            unitOfWorkDomainEventHandler2.Received().Execute(domainEvent, unitOfWork);
        }

        [TestMethod]
        public void ResolvedEventHandlersAreExecutedInCorrectOrder()
        {
            TestAggregateRoot aggregateRoot = new TestAggregateRoot(Guid.NewGuid());
            aggregateRoot.ApplyEvent(domainEvent);

            sut.ApplyChanges(aggregateRoot);

            Received.InOrder(() =>
            {
                unitOfWorkDomainEventHandler1.Execute(domainEvent, unitOfWork);
                unitOfWorkDomainEventHandler2.Execute(domainEvent, unitOfWork);
                domainEventHandler3.Execute(domainEvent);
                domainEventHandler4.Execute(domainEvent);
            });
        }

        [TestMethod]
        public void ChangesInUnitOfWorkAreSaved()
        {
            TestAggregateRoot aggregateRoot = new TestAggregateRoot(Guid.NewGuid());
            aggregateRoot.ApplyEvent(domainEvent);

            sut.ApplyChanges(aggregateRoot);

            unitOfWork.Received().SaveChanges();
        }

        [TestMethod]
        public void MultipleChangesInUnitOfWorkAreSavedOnce()
        {
            TestAggregateRoot aggregateRoot1 = new TestAggregateRoot(Guid.NewGuid());
            aggregateRoot1.ApplyEvent(domainEvent);

            TestDomainEvent domainEvent2 = new TestDomainEvent(Guid.NewGuid());
            unitOfWorkEventHandlerResolver.ResolveEventHandlers<IUnitOfWorkDomainEventHandler<TestDomainEvent, TestUnitOfWork>>().Returns(unitOfWorkDomainEventHandlers);

            TestAggregateRoot aggregateRoot2 = new TestAggregateRoot(Guid.NewGuid());
            aggregateRoot2.ApplyEvent(domainEvent2);

            sut.ApplyChanges(new List<TestAggregateRoot> { aggregateRoot1, aggregateRoot2 });

            Received.InOrder(() =>
            {
                unitOfWorkDomainEventHandler1.Execute(domainEvent, unitOfWork);
                unitOfWorkDomainEventHandler1.Execute(domainEvent2, unitOfWork);
                unitOfWork.SaveChanges();
            });
        }

        [TestMethod]
        public void OnlyUnitOfWorkEventsAreHandledIfCommitFails()
        {
            TestAggregateRoot aggregateRoot = new TestAggregateRoot(Guid.NewGuid());
            aggregateRoot.ApplyEvent(domainEvent);

            unitOfWork
                .When(u => u.SaveChanges())
                .Do(callInfo => throw new InvalidOperationException());

            try
            {
                sut.ApplyChanges(aggregateRoot);
                Assert.Fail("No exception thrown.");
            }
            catch (InvalidOperationException)
            {
            }

            unitOfWorkDomainEventHandler1.Received().Execute(domainEvent, unitOfWork);
            unitOfWorkDomainEventHandler2.Received().Execute(domainEvent, unitOfWork);
            unitOfWork.Received().SaveChanges();
            domainEventHandler3.DidNotReceive().Execute(domainEvent);
            domainEventHandler4.DidNotReceive().Execute(domainEvent);
        }
    }
}