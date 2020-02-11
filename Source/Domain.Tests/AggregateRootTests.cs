// ReSharper disable PossibleMultipleEnumeration

using System;
using System.Collections.Generic;
using System.Linq;
using Affecto.Patterns.Domain.Tests.TestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Affecto.Patterns.Domain.Tests
{
    [TestClass]
    public class AggregateRootTests
    {
        private Guid id;
        private TestAggregateRoot sut;

        [TestInitialize]
        public void Setup()
        {
            id = Guid.NewGuid();
            sut = new TestAggregateRoot(id);
        }

        [TestMethod]
        public void IdIsSet()
        {
            Assert.AreEqual(id, sut.Id);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void IdCannotBeDefaultEmpty()
        {
            sut = new TestAggregateRoot(Guid.Empty);
        }

        [TestMethod]
        public void VersionIsSet()
        {
            sut = new TestAggregateRoot(id, 10);
            Assert.AreEqual(10, sut.Version);
        }

        [TestMethod]
        public void VersionIsSetToDefault()
        {
            Assert.AreEqual(0, sut.Version);
        }

        [TestMethod]
        public void AppliedEventsIsEmpty()
        {
            IEnumerable<IDomainEvent> appliedEvents = sut.GetAppliedEvents();

            Assert.IsFalse(appliedEvents.Any());
        }

        [TestMethod]
        public void AppliedEventIsReturned()
        {
            var domainEvent = new TestDomainEvent(Guid.NewGuid());
            sut.ApplyEvent(domainEvent);
            IEnumerable<IDomainEvent> appliedEvents = sut.GetAppliedEvents();

            Assert.AreEqual(1, appliedEvents.Count());
            Assert.AreSame(domainEvent, appliedEvents.Single());
        }

        [TestMethod]
        public void AppliedEventsAreReturnedInAppliedOrder()
        {
            var domainEvent1 = new TestDomainEvent(Guid.NewGuid());
            var domainEvent2 = new TestDomainEvent(Guid.NewGuid());
            sut.ApplyEvent(domainEvent2);
            sut.ApplyEvent(domainEvent1);

            IEnumerable<IDomainEvent> appliedEvents = sut.GetAppliedEvents();

            Assert.AreEqual(2, appliedEvents.Count());
            Assert.AreSame(domainEvent2, appliedEvents.ElementAt(0));
            Assert.AreSame(domainEvent1, appliedEvents.ElementAt(1));
        }

        [TestMethod]
        public void VersionIsSetToPreviouslyGeneratedEvents()
        {
            const long expectedVersion = 313;

            var domainEvent1 = new TestDomainEvent(Guid.NewGuid(), 1);
            var domainEvent2 = new TestDomainEvent(Guid.NewGuid(), 2);
            sut.ApplyEvent(domainEvent2);
            sut.ApplyEvent(domainEvent1);

            sut.SetVersion(expectedVersion);

            Assert.AreEqual(expectedVersion, domainEvent1.EntityVersion);
            Assert.AreEqual(expectedVersion, domainEvent2.EntityVersion);
        }

        [TestMethod]
        public void VersionIsSetToNewGeneratedEvents()
        {
            const long expectedVersion = 313;

            sut.SetVersion(expectedVersion);

            var domainEvent1 = new TestDomainEvent(Guid.NewGuid(), 1);
            var domainEvent2 = new TestDomainEvent(Guid.NewGuid(), 2);
            sut.ApplyEvent(domainEvent2);
            sut.ApplyEvent(domainEvent1);

            Assert.AreEqual(expectedVersion, domainEvent1.EntityVersion);
            Assert.AreEqual(expectedVersion, domainEvent2.EntityVersion);
        }
    }
}