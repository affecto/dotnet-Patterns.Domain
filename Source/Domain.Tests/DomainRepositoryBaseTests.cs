using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        public async Task NoEventsExecutedIfNoAppliedEventsExist()
        {
            TestAggregateRoot aggregateRoot = new TestAggregateRoot(Guid.NewGuid());
            await sut.ApplyChangesAsync(aggregateRoot);

            Assert.AreEqual(0, sut.EventBroker.ExecutedEvents.Count);
        }

        [TestMethod]
        public async Task AllAppliedEventsAreExecuted()
        {
            TestAggregateRoot aggregateRoot = new TestAggregateRoot(Guid.NewGuid());
            var domainEvent1 = ApplyNewEvent(aggregateRoot);
            var domainEvent2 = ApplyNewEvent(aggregateRoot);

            await sut.ApplyChangesAsync(aggregateRoot);

            Assert.AreEqual(2, sut.EventBroker.ExecutedEvents.Count);
            Assert.IsTrue(sut.EventBroker.ExecutedEvents.Contains(domainEvent1));
            Assert.IsTrue(sut.EventBroker.ExecutedEvents.Contains(domainEvent2));
        }

        [TestMethod]
        public async Task AllAppliedEventsForAllAggregateRootsAreExecuted()
        {
            TestAggregateRoot aggregateRoot1 = new TestAggregateRoot(Guid.NewGuid());
            var domainEvent1 = ApplyNewEvent(aggregateRoot1);
            var domainEvent2 = ApplyNewEvent(aggregateRoot1);

            TestAggregateRoot aggregateRoot2 = new TestAggregateRoot(Guid.NewGuid());
            var domainEvent3 = ApplyNewEvent(aggregateRoot2);
            var domainEvent4 = ApplyNewEvent(aggregateRoot2);

            await sut.ApplyChangesAsync(new List<TestAggregateRoot> { aggregateRoot1, aggregateRoot2 });

            Assert.AreEqual(4, sut.EventBroker.ExecutedEvents.Count);
            Assert.IsTrue(sut.EventBroker.ExecutedEvents.Contains(domainEvent1));
            Assert.IsTrue(sut.EventBroker.ExecutedEvents.Contains(domainEvent2));
            Assert.IsTrue(sut.EventBroker.ExecutedEvents.Contains(domainEvent3));
            Assert.IsTrue(sut.EventBroker.ExecutedEvents.Contains(domainEvent4));
        }

        private static DomainEvent ApplyNewEvent(TestAggregateRoot aggregateRoot)
        {
            var domainEvent = new TestDomainEvent(Guid.NewGuid());
            aggregateRoot.AddEvent(domainEvent);
            return domainEvent;
        }
    }
}