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
        public async Task NullAggregateThrowsException()
        {
            await sut.ApplyChangesAsync((TestAggregateRoot) null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task NullAggregatesThrowsException()
        {
            await sut.ApplyChangesAsync((IReadOnlyCollection<TestAggregateRoot>) null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task SomeOfAggregatesNullThrowsException()
        {
            TestAggregateRoot aggregateRoot1 = new TestAggregateRoot(Guid.NewGuid());
            TestAggregateRoot aggregateRoot2 = new TestAggregateRoot(Guid.NewGuid());

            await sut.ApplyChangesAsync(new List<TestAggregateRoot> { aggregateRoot1, null, aggregateRoot2 });
        }

        [TestMethod]
        public async Task ResolvedUnitOfWorkEventHandlersAreExecutedInCorrectOrder()
        {
            TestAggregateRoot aggregateRoot = new TestAggregateRoot(Guid.NewGuid());
            aggregateRoot.AddEvent(domainEvent);

            await sut.ApplyChangesAsync(aggregateRoot);

            Received.InOrder(() =>
            {
                unitOfWorkDomainEventHandler1.ExecuteAsync(domainEvent, unitOfWork);
                unitOfWorkDomainEventHandler2.ExecuteAsync(domainEvent, unitOfWork);
            });
        }

        [TestMethod]
        public async Task ResolvedEventHandlersAreExecutedInCorrectOrder()
        {
            TestAggregateRoot aggregateRoot = new TestAggregateRoot(Guid.NewGuid());
            aggregateRoot.AddEvent(domainEvent);

            await sut.ApplyChangesAsync(aggregateRoot);

            Received.InOrder(() =>
            {
                unitOfWorkDomainEventHandler1.ExecuteAsync(domainEvent, unitOfWork);
                unitOfWorkDomainEventHandler2.ExecuteAsync(domainEvent, unitOfWork);
                domainEventHandler3.ExecuteAsync(domainEvent);
                domainEventHandler4.ExecuteAsync(domainEvent);
            });
        }

        [TestMethod]
        public async Task ChangesInUnitOfWorkAreSaved()
        {
            TestAggregateRoot aggregateRoot = new TestAggregateRoot(Guid.NewGuid());
            aggregateRoot.AddEvent(domainEvent);

            await sut.ApplyChangesAsync(aggregateRoot);

            await unitOfWork.Received().SaveChangesAsync();
        }

        [TestMethod]
        public async Task MultipleChangesInUnitOfWorkAreSavedOnce()
        {
            TestAggregateRoot aggregateRoot1 = new TestAggregateRoot(Guid.NewGuid());
            aggregateRoot1.AddEvent(domainEvent);

            TestAggregateRoot aggregateRoot2 = new TestAggregateRoot(Guid.NewGuid());
            TestDomainEvent domainEvent2 = new TestDomainEvent(Guid.NewGuid());
            aggregateRoot2.AddEvent(domainEvent2);

            await sut.ApplyChangesAsync(new List<TestAggregateRoot> { aggregateRoot1, aggregateRoot2 });

            Received.InOrder(() =>
            {
                unitOfWorkDomainEventHandler1.ExecuteAsync(domainEvent, unitOfWork);
                unitOfWorkDomainEventHandler1.ExecuteAsync(domainEvent2, unitOfWork);
                unitOfWork.SaveChangesAsync();
            });

            await unitOfWork.Received(1).SaveChangesAsync();
        }

        [TestMethod]
        public async Task OnlyUnitOfWorkEventsAreHandledIfCommitFails()
        {
            TestAggregateRoot aggregateRoot = new TestAggregateRoot(Guid.NewGuid());
            aggregateRoot.AddEvent(domainEvent);

            unitOfWork
                .When(u => u.SaveChangesAsync())
                .Do(callInfo => throw new InvalidOperationException());

            try
            {
                await sut.ApplyChangesAsync(aggregateRoot);
                Assert.Fail("No exception thrown.");
            }
            catch (InvalidOperationException)
            {
            }

            Received.InOrder(() =>
            {
                unitOfWorkDomainEventHandler1.ExecuteAsync(domainEvent, unitOfWork);
                unitOfWorkDomainEventHandler2.ExecuteAsync(domainEvent, unitOfWork);
                unitOfWork.SaveChangesAsync();
            });

            await domainEventHandler3.DidNotReceive().ExecuteAsync(domainEvent);
            await domainEventHandler4.DidNotReceive().ExecuteAsync(domainEvent);
        }
    }
}