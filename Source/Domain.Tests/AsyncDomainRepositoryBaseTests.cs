using System;
using System.Collections.Generic;
using Affecto.Patterns.Domain.Tests.Infrastructure;
using Affecto.Patterns.Domain.Tests.TestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Affecto.Patterns.Domain.Tests
{
    [TestClass]
    public class AsyncDomainRepositoryBaseTests
    {
        private TestAsyncDomainRepositoryBase sut;

        [TestInitialize]
        public void Setup()
        {
            sut = new TestAsyncDomainRepositoryBase();
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
        public void NoEventsExecutedIfNoAppliedEventsExist()
        {
            TestAggregateRoot aggregateRoot = new TestAggregateRoot(Guid.NewGuid());
            sut.ApplyChangesAsync(aggregateRoot).Wait();

            Assert.AreEqual(0, sut.EventBroker.ExecutedEvents.Count);
        }

        [TestMethod]
        public void AllAppliedEventsAreExecuted()
        {
            TestAggregateRoot aggregateRoot = new TestAggregateRoot(Guid.NewGuid());
            var domainEvent1 = ApplyNewEvent(aggregateRoot);
            var domainEvent2 = ApplyNewEvent(aggregateRoot);

            sut.ApplyChangesAsync(aggregateRoot).Wait();

            Assert.AreEqual(2, sut.EventBroker.ExecutedEvents.Count);
            Assert.IsTrue(sut.EventBroker.ExecutedEvents.Contains(domainEvent1));
            Assert.IsTrue(sut.EventBroker.ExecutedEvents.Contains(domainEvent2));
        }

        [TestMethod]
        public void AllAppliedEventsForAllAggregateRootsAreExecuted()
        {
            TestAggregateRoot aggregateRoot1 = new TestAggregateRoot(Guid.NewGuid());
            var domainEvent1 = ApplyNewEvent(aggregateRoot1);
            var domainEvent2 = ApplyNewEvent(aggregateRoot1);

            TestAggregateRoot aggregateRoot2 = new TestAggregateRoot(Guid.NewGuid());
            var domainEvent3 = ApplyNewEvent(aggregateRoot2);
            var domainEvent4 = ApplyNewEvent(aggregateRoot2);

            sut.ApplyChangesAsync(new List<TestAggregateRoot> { aggregateRoot1, aggregateRoot2 }).Wait();

            Assert.AreEqual(4, sut.EventBroker.ExecutedEvents.Count);
            Assert.IsTrue(sut.EventBroker.ExecutedEvents.Contains(domainEvent1));
            Assert.IsTrue(sut.EventBroker.ExecutedEvents.Contains(domainEvent2));
            Assert.IsTrue(sut.EventBroker.ExecutedEvents.Contains(domainEvent3));
            Assert.IsTrue(sut.EventBroker.ExecutedEvents.Contains(domainEvent4));
        }

        private static DomainEvent ApplyNewEvent(TestAggregateRoot aggregateRoot)
        {
            var domainEvent = new TestDomainEvent(Guid.NewGuid());
            aggregateRoot.ApplyEvent(domainEvent);
            return domainEvent;
        }
    }
}