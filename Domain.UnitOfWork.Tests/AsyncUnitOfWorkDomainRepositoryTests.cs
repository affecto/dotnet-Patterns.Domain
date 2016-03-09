using System;
using System.Collections.Generic;
using Affecto.Patterns.Domain.Tests.Infrastructure;
using Affecto.Patterns.Domain.Tests.TestHelpers;
using Affecto.Patterns.Domain.UnitOfWork.Tests.TestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Affecto.Patterns.Domain.UnitOfWork.Tests
{
    [TestClass]
    public class AsyncUnitOfWorkDomainRepositoryTests
    {
        private IAsyncUnitOfWorkDomainEventHandler<TestDomainEvent, TestUnitOfWork> unitOfWorkDomainEventHandler1;
        private IAsyncUnitOfWorkDomainEventHandler<TestDomainEvent, TestUnitOfWork> unitOfWorkDomainEventHandler2;
        private IAsyncDomainEventHandler<TestDomainEvent> domainEventHandler3;
        private IAsyncDomainEventHandler<TestDomainEvent> domainEventHandler4;

        private IEnumerable<IAsyncUnitOfWorkDomainEventHandler<TestDomainEvent, TestUnitOfWork>> unitOfWorkDomainEventHandlers;
        private IEnumerable<IAsyncDomainEventHandler<TestDomainEvent>> domainEventHandlers;

        private TestDomainEvent domainEvent;

        private IDomainEventHandlerResolver eventHandlerResolver;
        private IUnitOfWorkDomainEventHandlerResolver unitOfWorkEventHandlerResolver;
        private TestUnitOfWork unitOfWork;

        private TestAsyncUnitOfWorkDomainRepository sut;

        [TestInitialize]
        public void Setup()
        {
            unitOfWorkDomainEventHandler1 = Substitute.For<IAsyncUnitOfWorkDomainEventHandler<TestDomainEvent, TestUnitOfWork>>();
            unitOfWorkDomainEventHandler2 = Substitute.For<IAsyncUnitOfWorkDomainEventHandler<TestDomainEvent, TestUnitOfWork>>();
            
            unitOfWorkDomainEventHandlers = new List<IAsyncUnitOfWorkDomainEventHandler<TestDomainEvent, TestUnitOfWork>>
            {
                unitOfWorkDomainEventHandler1,
                unitOfWorkDomainEventHandler2
            };

            domainEventHandler3 = Substitute.For<IAsyncDomainEventHandler<TestDomainEvent>>();
            domainEventHandler4 = Substitute.For<IAsyncDomainEventHandler<TestDomainEvent>>();
            domainEventHandlers = new List<IAsyncDomainEventHandler<TestDomainEvent>>
            {
                domainEventHandler3,
                domainEventHandler4
            };

            eventHandlerResolver = Substitute.For<IDomainEventHandlerResolver>();
            unitOfWorkEventHandlerResolver = Substitute.For<IUnitOfWorkDomainEventHandlerResolver>();
            unitOfWork = Substitute.For<TestUnitOfWork>();

            domainEvent = new TestDomainEvent(Guid.NewGuid());
            unitOfWorkEventHandlerResolver.ResolveEventHandlers<IAsyncUnitOfWorkDomainEventHandler<TestDomainEvent, TestUnitOfWork>>().Returns(unitOfWorkDomainEventHandlers);
            eventHandlerResolver.ResolveEventHandlers<IAsyncDomainEventHandler<TestDomainEvent>>().Returns(domainEventHandlers);

            sut = new TestAsyncUnitOfWorkDomainRepository(eventHandlerResolver, unitOfWorkEventHandlerResolver, unitOfWork);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullUnitOfWorkThrowsException()
        {
            sut = new TestAsyncUnitOfWorkDomainRepository(eventHandlerResolver, unitOfWorkEventHandlerResolver, null);
        }

        [TestMethod]
        [ExpectedAggregateException(typeof(ArgumentNullException))]
        public void NullAggregateThrowsException()
        {
            sut.ApplyChangesAsync((TestAggregateRoot) null).Wait();
        }

        [TestMethod]
        [ExpectedAggregateException(typeof(ArgumentNullException))]
        public void NullAggregatesThrowsException()
        {
            sut.ApplyChangesAsync((IEnumerable<TestAggregateRoot>) null).Wait();
        }

        [TestMethod]
        public void ResolvedUnitOfWorkEventHandlersAreExecutedInCorrectOrder()
        {
            TestAggregateRoot aggregateRoot = new TestAggregateRoot(Guid.NewGuid());
            aggregateRoot.ApplyEvent(domainEvent);

            sut.ApplyChangesAsync(aggregateRoot).Wait();

            unitOfWorkDomainEventHandler1.Received().ExecuteAsync(domainEvent, unitOfWork);
            unitOfWorkDomainEventHandler2.Received().ExecuteAsync(domainEvent, unitOfWork);
        }

        [TestMethod]
        public void ResolvedEventHandlersAreExecutedInCorrectOrder()
        {
            TestAggregateRoot aggregateRoot = new TestAggregateRoot(Guid.NewGuid());
            aggregateRoot.ApplyEvent(domainEvent);

            sut.ApplyChangesAsync(aggregateRoot).Wait();

            Received.InOrder(() =>
            {
                unitOfWorkDomainEventHandler1.ExecuteAsync(domainEvent, unitOfWork);
                unitOfWorkDomainEventHandler2.ExecuteAsync(domainEvent, unitOfWork);
                domainEventHandler3.ExecuteAsync(domainEvent);
                domainEventHandler4.ExecuteAsync(domainEvent);
            });
        }

        [TestMethod]
        public void ChangesInUnitOfWorkAreSaved()
        {
            TestAggregateRoot aggregateRoot = new TestAggregateRoot(Guid.NewGuid());
            aggregateRoot.ApplyEvent(domainEvent);

            sut.ApplyChangesAsync(aggregateRoot).Wait();

            unitOfWork.Received().SaveChanges();
        }

        [TestMethod]
        public void MultipleChangesInUnitOfWorkAreSavedOnce()
        {
            TestAggregateRoot aggregateRoot1 = new TestAggregateRoot(Guid.NewGuid());
            aggregateRoot1.ApplyEvent(domainEvent);

            TestDomainEvent domainEvent2 = new TestDomainEvent(Guid.NewGuid());
            unitOfWorkEventHandlerResolver.ResolveEventHandlers<IAsyncUnitOfWorkDomainEventHandler<TestDomainEvent, TestUnitOfWork>>().Returns(unitOfWorkDomainEventHandlers);

            TestAggregateRoot aggregateRoot2 = new TestAggregateRoot(Guid.NewGuid());
            aggregateRoot2.ApplyEvent(domainEvent2);

            sut.ApplyChangesAsync(new List<TestAggregateRoot> { aggregateRoot1, aggregateRoot2 }).Wait();

            Received.InOrder(() =>
            {
                unitOfWorkDomainEventHandler1.ExecuteAsync(domainEvent, unitOfWork);
                unitOfWorkDomainEventHandler1.ExecuteAsync(domainEvent2, unitOfWork);
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
                .Do(callInfo => { throw new InvalidOperationException(); });

            try
            {
                sut.ApplyChangesAsync(aggregateRoot).Wait();
                Assert.Fail("No exception thrown.");
            }
            catch (AggregateException e)
            {
                if (e.InnerException == null)
                {
                    Assert.Fail("Expected inner exception of type AggregateException. No inner exception received.");
                }

                if (!(e.InnerException is InvalidOperationException))
                {
                    Assert.Fail("Expected inner exception of type AggregateException. Received '{0}'.", e.InnerException);
                }
            }

            unitOfWorkDomainEventHandler1.Received().ExecuteAsync(domainEvent, unitOfWork);
            unitOfWorkDomainEventHandler2.Received().ExecuteAsync(domainEvent, unitOfWork);
            unitOfWork.Received().SaveChanges();
            domainEventHandler3.DidNotReceive().ExecuteAsync(domainEvent);
            domainEventHandler4.DidNotReceive().ExecuteAsync(domainEvent);
        }
    }
}