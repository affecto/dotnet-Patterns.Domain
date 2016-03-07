using System;
using System.Collections.Generic;
using Affecto.Patterns.Domain.Tests.TestHelpers;
using Affecto.Patterns.Domain.UnitOfWork.Tests.TestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Affecto.Patterns.Domain.UnitOfWork.Tests
{
    [TestClass]
    public class CompositeUnitOfWorkDomainRepositoryTests
    {
        private IUnitOfWorkDomainEventHandler<TestDomainEvent, TestUnitOfWork> unitOfWorkDomainEventHandler1;
        private IUnitOfWorkDomainEventHandler<TestDomainEvent, TestUnitOfWork> unitOfWorkDomainEventHandler2;
        private IDomainEventHandler<TestDomainEvent> domainEventHandler3;
        private IDomainEventHandler<TestDomainEvent> domainEventHandler4;

        private IEnumerable<IUnitOfWorkDomainEventHandler<TestDomainEvent, TestUnitOfWork>> unitOfWorkDomainEventHandlers;
        private IEnumerable<IDomainEventHandler<TestDomainEvent>> domainEventHandlers;

        private TestDomainEvent domainEvent1;
        private TestDomainEvent domainEvent2;

        private IDomainEventHandlerResolver eventHandlerResolver;
        private IUnitOfWorkDomainEventHandlerResolver unitOfWorkEventHandlerResolver;
        private TestUnitOfWork unitOfWork;

        private TestCompositeUnitOfWorkDomainRepository sut;

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

            domainEvent1 = new TestDomainEvent(Guid.NewGuid());
            domainEvent2 = new TestDomainEvent(Guid.NewGuid());
            unitOfWorkEventHandlerResolver.ResolveEventHandlers<TestDomainEvent, TestUnitOfWork>(domainEvent1).Returns(unitOfWorkDomainEventHandlers);
            unitOfWorkEventHandlerResolver.ResolveEventHandlers<TestDomainEvent, TestUnitOfWork>(domainEvent2).Returns(unitOfWorkDomainEventHandlers);
            eventHandlerResolver.ResolveEventHandlers(domainEvent1).Returns(domainEventHandlers);
            eventHandlerResolver.ResolveEventHandlers(domainEvent2).Returns(domainEventHandlers);

            sut = new TestCompositeUnitOfWorkDomainRepository(eventHandlerResolver, unitOfWorkEventHandlerResolver, unitOfWork);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullUnitOfWorkThrowsException()
        {
            sut = new TestCompositeUnitOfWorkDomainRepository(eventHandlerResolver, unitOfWorkEventHandlerResolver, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullFirstAggregate1ThrowsException()
        {
            sut.ApplyChanges(null, new TestAggregateRoot2(Guid.NewGuid()));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullSecondAggregate2ThrowsException()
        {
            sut.ApplyChanges(new TestAggregateRoot(Guid.NewGuid()), null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullFirstAggregate2ThrowsException()
        {
            sut.ApplyChanges(null, new TestAggregateRoot(Guid.NewGuid()));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullSecondAggregate1ThrowsException()
        {
            sut.ApplyChanges(new TestAggregateRoot2(Guid.NewGuid()), null);
        }

        [TestMethod]
        [ExpectedException(typeof(IncompatibleAggregateRootTypeException))]
        public void IncompatibleAggregateRootTypeCannotBeUsed()
        {
            sut.Find<TestAggregateRoot3>(Guid.NewGuid());
        }

        [TestMethod]
        public void AggregateRootOfFirstSpecifiedTypeIsFound()
        {
            TestAggregateRoot aggregateRoot = sut.Find<TestAggregateRoot>(Guid.NewGuid());
            Assert.IsNotNull(aggregateRoot);
        }

        [TestMethod]
        public void AggregateRootOfSecondSpecifiedTypeIsFound()
        {
            TestAggregateRoot2 aggregateRoot = sut.Find<TestAggregateRoot2>(Guid.NewGuid());
            Assert.IsNotNull(aggregateRoot);
        }

        [TestMethod]
        public void ResolvedEventHandlersAreExecutedInCorrectOrder()
        {
            TestAggregateRoot aggregateRoot = new TestAggregateRoot(Guid.NewGuid());
            aggregateRoot.ApplyEvent(domainEvent1);

            TestAggregateRoot2 aggregateRoot2 = new TestAggregateRoot2(Guid.NewGuid());
            aggregateRoot.ApplyEvent(domainEvent2);

            sut.ApplyChanges(aggregateRoot, aggregateRoot2);

            Received.InOrder(() =>
            {
                unitOfWorkDomainEventHandler1.Execute(domainEvent1, unitOfWork);
                unitOfWorkDomainEventHandler2.Execute(domainEvent1, unitOfWork);
                unitOfWorkDomainEventHandler1.Execute(domainEvent2, unitOfWork);
                unitOfWorkDomainEventHandler2.Execute(domainEvent2, unitOfWork);
                unitOfWork.SaveChanges();
                domainEventHandler3.Execute(domainEvent1);
                domainEventHandler4.Execute(domainEvent1);
                domainEventHandler3.Execute(domainEvent2);
                domainEventHandler4.Execute(domainEvent2);
            });
        }

        [TestMethod]
        public void ChangesInUnitOfWorkAreSavedOnce()
        {
            TestAggregateRoot aggregateRoot = new TestAggregateRoot(Guid.NewGuid());
            aggregateRoot.ApplyEvent(domainEvent1);

            TestAggregateRoot2 aggregateRoot2 = new TestAggregateRoot2(Guid.NewGuid());
            aggregateRoot.ApplyEvent(domainEvent2);

            sut.ApplyChanges(aggregateRoot, aggregateRoot2);

            unitOfWork.Received(1).SaveChanges();
        }

        [TestMethod]
        public void InaccessibleTypeParametersCanBeUsed()
        {
            // This test is necessary for checking that the repository can be used with inaccessible type parameters.
            // See the bug in .NET Framework: https://connect.microsoft.com/VisualStudio/feedback/details/672411/bug-in-dynamic-dispatch-when-using-generic-type-parameters

            IUnitOfWorkDomainEventHandler<TestDomainEvent, InternalUnitOfWork> domainEventHandler =
                Substitute.For<IUnitOfWorkDomainEventHandler<TestDomainEvent, InternalUnitOfWork>>();
            IEnumerable<IUnitOfWorkDomainEventHandler<TestDomainEvent, InternalUnitOfWork>> eventHandlers = new List
                <IUnitOfWorkDomainEventHandler<TestDomainEvent, InternalUnitOfWork>>
            {
                domainEventHandler
            };

            unitOfWorkEventHandlerResolver.ResolveEventHandlers<TestDomainEvent, InternalUnitOfWork>(domainEvent1).Returns(eventHandlers);
            unitOfWorkEventHandlerResolver.ResolveEventHandlers<TestDomainEvent, InternalUnitOfWork>(domainEvent2).Returns(eventHandlers);

            InternalAggregateRoot aggregateRoot = new InternalAggregateRoot(Guid.NewGuid());
            aggregateRoot.ApplyEvent(domainEvent1);

            TestAggregateRoot2 aggregateRoot2 = new TestAggregateRoot2(Guid.NewGuid());
            aggregateRoot.ApplyEvent(domainEvent2);

            InternalUnitOfWork internalUnitOfWork = new InternalUnitOfWork();

            var privateSut = new InternalCompositeRepository(eventHandlerResolver, unitOfWorkEventHandlerResolver, internalUnitOfWork);
            privateSut.ApplyChanges(aggregateRoot, aggregateRoot2);

            Received.InOrder(() =>
            {
                domainEventHandler.Execute(domainEvent1, internalUnitOfWork);
                domainEventHandler.Execute(domainEvent2, internalUnitOfWork);
                internalUnitOfWork.SaveChanges();
            });
        }

        [TestMethod]
        public void OnlyUnitOfWorkEventsAreHandledIfCommitFails()
        {
            TestAggregateRoot aggregateRoot = new TestAggregateRoot(Guid.NewGuid());
            aggregateRoot.ApplyEvent(domainEvent1);

            TestAggregateRoot2 aggregateRoot2 = new TestAggregateRoot2(Guid.NewGuid());
            aggregateRoot.ApplyEvent(domainEvent2);

            unitOfWork
                .When(u => u.SaveChanges())
                .Do(callInfo => { throw new OperationCanceledException(); });

            bool exceptionCatched = false;

            try
            {
                sut.ApplyChanges(aggregateRoot, aggregateRoot2);
            }
            catch (OperationCanceledException)
            {
                exceptionCatched = true;
            }

            Assert.IsTrue(exceptionCatched);
            unitOfWorkDomainEventHandler1.Received().Execute(domainEvent1, unitOfWork);
            unitOfWorkDomainEventHandler2.Received().Execute(domainEvent1, unitOfWork);
            unitOfWorkDomainEventHandler1.Received().Execute(domainEvent2, unitOfWork);
            unitOfWorkDomainEventHandler2.Received().Execute(domainEvent2, unitOfWork);
            unitOfWork.Received().SaveChanges();
            domainEventHandler3.DidNotReceive().Execute(domainEvent1);
            domainEventHandler4.DidNotReceive().Execute(domainEvent1);
            domainEventHandler3.DidNotReceive().Execute(domainEvent2);
            domainEventHandler4.DidNotReceive().Execute(domainEvent2);
        }
    }
}