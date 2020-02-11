using System;
using System.Collections.Generic;
using Affecto.Patterns.Domain.Tests.TestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Affecto.Patterns.Domain.Tests
{
    [TestClass]
    public class DomainRepositoryBaseTests
    {
        private TestDomainRepositoryBase sut;

        [TestInitialize]
        public void Setup()
        {
            sut = new TestDomainRepositoryBase();
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
        public void NoEventsExecutedIfNoAppliedEventsExist()
        {
            TestAggregateRoot aggregateRoot = new TestAggregateRoot(Guid.NewGuid());
            sut.ApplyChanges(aggregateRoot);

            Assert.AreEqual(0, sut.EventBroker.ExecutedEvents.Count);
        }

        [TestMethod]
        public void AllAppliedEventsAreExecuted()
        {
            TestAggregateRoot aggregateRoot = new TestAggregateRoot(Guid.NewGuid());
            var domainEvent1 = ApplyNewEvent(aggregateRoot);
            var domainEvent2 = ApplyNewEvent(aggregateRoot);

            sut.ApplyChanges(aggregateRoot);

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

            sut.ApplyChanges(new List<TestAggregateRoot> { aggregateRoot1, aggregateRoot2 });

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