using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
            unitOfWorkEventHandlerResolver
                .ResolveEventHandlers<IUnitOfWorkDomainEventHandler<TestDomainEvent, TestUnitOfWork>>().Returns(unitOfWorkDomainEventHandlers);
            unitOfWorkEventHandlerResolver.ResolveEventHandlers<IUnitOfWorkDomainEventHandler<TestDomainEvent, TestUnitOfWork>>().Returns(unitOfWorkDomainEventHandlers);
            eventHandlerResolver.ResolveEventHandlers<IDomainEventHandler<TestDomainEvent>>().Returns(domainEventHandlers);
            eventHandlerResolver.ResolveEventHandlers<IDomainEventHandler<TestDomainEvent>>().Returns(domainEventHandlers);

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
        public async Task NullFirstAggregate1ThrowsException()
        {
            await sut.ApplyChangesAsync(null, new TestAggregateRoot2(Guid.NewGuid()));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task NullSecondAggregate2ThrowsException()
        {
            await sut.ApplyChangesAsync(new TestAggregateRoot(Guid.NewGuid()), null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task NullFirstAggregate2ThrowsException()
        {
            await sut.ApplyChangesAsync(null, new TestAggregateRoot(Guid.NewGuid()));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task NullSecondAggregate1ThrowsException()
        {
            await sut.ApplyChangesAsync(new TestAggregateRoot2(Guid.NewGuid()), null);
        }

        [TestMethod]
        [ExpectedException(typeof(IncompatibleAggregateRootTypeException))]
        public async Task IncompatibleAggregateRootTypeCannotBeUsed()
        {
            await sut.FindAsync<TestAggregateRoot3>(Guid.NewGuid());
        }

        [TestMethod]
        public async Task AggregateRootOfFirstSpecifiedTypeIsFound()
        {
            TestAggregateRoot aggregateRoot = await sut.FindAsync<TestAggregateRoot>(Guid.NewGuid());
            Assert.IsNotNull(aggregateRoot);
        }

        [TestMethod]
        public async Task AggregateRootOfSecondSpecifiedTypeIsFound()
        {
            TestAggregateRoot2 aggregateRoot = await sut.FindAsync<TestAggregateRoot2>(Guid.NewGuid());
            Assert.IsNotNull(aggregateRoot);
        }

        [TestMethod]
        public async Task ResolvedEventHandlersAreExecutedInCorrectOrder()
        {
            TestAggregateRoot aggregateRoot = new TestAggregateRoot(Guid.NewGuid());
            aggregateRoot.AddEvent(domainEvent1);

            TestAggregateRoot2 aggregateRoot2 = new TestAggregateRoot2(Guid.NewGuid());
            aggregateRoot.AddEvent(domainEvent2);

            await sut.ApplyChangesAsync(aggregateRoot, aggregateRoot2);

            Received.InOrder(() =>
            {
                unitOfWorkDomainEventHandler1.ExecuteAsync(domainEvent1, unitOfWork);
                unitOfWorkDomainEventHandler2.ExecuteAsync(domainEvent1, unitOfWork);
                unitOfWorkDomainEventHandler1.ExecuteAsync(domainEvent2, unitOfWork);
                unitOfWorkDomainEventHandler2.ExecuteAsync(domainEvent2, unitOfWork);
                unitOfWork.SaveChangesAsync();
                domainEventHandler3.ExecuteAsync(domainEvent1);
                domainEventHandler4.ExecuteAsync(domainEvent1);
                domainEventHandler3.ExecuteAsync(domainEvent2);
                domainEventHandler4.ExecuteAsync(domainEvent2);
            });
        }

        [TestMethod]
        public async Task ChangesInUnitOfWorkAreSavedOnce()
        {
            TestAggregateRoot aggregateRoot = new TestAggregateRoot(Guid.NewGuid());
            aggregateRoot.AddEvent(domainEvent1);

            TestAggregateRoot2 aggregateRoot2 = new TestAggregateRoot2(Guid.NewGuid());
            aggregateRoot.AddEvent(domainEvent2);

            await sut.ApplyChangesAsync(aggregateRoot, aggregateRoot2);

            await unitOfWork.Received(1).SaveChangesAsync();
        }

        [TestMethod]
        public async Task InaccessibleTypeParametersCanBeUsed()
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

            unitOfWorkEventHandlerResolver.ResolveEventHandlers<IUnitOfWorkDomainEventHandler<TestDomainEvent, InternalUnitOfWork>>().Returns(eventHandlers);
            unitOfWorkEventHandlerResolver.ResolveEventHandlers<IUnitOfWorkDomainEventHandler<TestDomainEvent, InternalUnitOfWork>>().Returns(eventHandlers);

            InternalAggregateRoot aggregateRoot = new InternalAggregateRoot(Guid.NewGuid());
            aggregateRoot.AddEvent(domainEvent1);

            TestAggregateRoot2 aggregateRoot2 = new TestAggregateRoot2(Guid.NewGuid());
            aggregateRoot.AddEvent(domainEvent2);

            InternalUnitOfWork internalUnitOfWork = new InternalUnitOfWork();

            var privateSut = new InternalCompositeRepository(eventHandlerResolver, unitOfWorkEventHandlerResolver, internalUnitOfWork);
            await privateSut.ApplyChangesAsync(aggregateRoot, aggregateRoot2);

            Received.InOrder(() =>
            {
                domainEventHandler.ExecuteAsync(domainEvent1, internalUnitOfWork);
                domainEventHandler.ExecuteAsync(domainEvent2, internalUnitOfWork);
                internalUnitOfWork.SaveChangesAsync();
            });
        }

        [TestMethod]
        public async Task OnlyUnitOfWorkEventsAreHandledIfCommitFails()
        {
            TestAggregateRoot aggregateRoot = new TestAggregateRoot(Guid.NewGuid());
            aggregateRoot.AddEvent(domainEvent1);

            TestAggregateRoot2 aggregateRoot2 = new TestAggregateRoot2(Guid.NewGuid());
            aggregateRoot.AddEvent(domainEvent2);

            unitOfWork
                .When(u => u.SaveChangesAsync())
                .Do(callInfo => throw new OperationCanceledException());

            bool exceptionCaught = false;

            try
            {
                await sut.ApplyChangesAsync(aggregateRoot, aggregateRoot2);
            }
            catch (OperationCanceledException)
            {
                exceptionCaught = true;
            }

            Assert.IsTrue(exceptionCaught);
            await unitOfWorkDomainEventHandler1.Received().ExecuteAsync(domainEvent1, unitOfWork);
            await unitOfWorkDomainEventHandler2.Received().ExecuteAsync(domainEvent1, unitOfWork);
            await unitOfWorkDomainEventHandler1.Received().ExecuteAsync(domainEvent2, unitOfWork);
            await unitOfWorkDomainEventHandler2.Received().ExecuteAsync(domainEvent2, unitOfWork);
            await unitOfWork.Received().SaveChangesAsync();
            await domainEventHandler3.DidNotReceive().ExecuteAsync(domainEvent1);
            await domainEventHandler4.DidNotReceive().ExecuteAsync(domainEvent1);
            await domainEventHandler3.DidNotReceive().ExecuteAsync(domainEvent2);
            await domainEventHandler4.DidNotReceive().ExecuteAsync(domainEvent2);
        }
    }
}