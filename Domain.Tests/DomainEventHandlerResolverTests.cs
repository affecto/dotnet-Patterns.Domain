// ReSharper disable ObjectCreationAsStatement
// ReSharper disable PossibleMultipleEnumeration

using System;
using System.Collections.Generic;
using System.Linq;
using Affecto.Patterns.Domain.Tests.TestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Affecto.Patterns.Domain.Tests
{
    [TestClass]
    public class DomainEventHandlerResolverTests
    {
        private TestDomainEvent domainEvent;
        private List<IDomainEventHandler<IDomainEvent>> eventHandlers;
        private DomainEventHandlerResolver sut;

        [TestInitialize]
        public void Setup()
        {
            domainEvent = new TestDomainEvent(Guid.NewGuid());
            eventHandlers =  new List<IDomainEventHandler<IDomainEvent>>();

            sut = new DomainEventHandlerResolver(eventHandlers);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void EventHandlersCannotBeNull()
        {
            new DomainEventHandlerResolver(null);
        }

        [TestMethod]
        public void SingleRegisteredEventHandlerIsReturned()
        {
            IDomainEventHandler<IDomainEvent> eventHandler = SetupEventHandler();

            IEnumerable<IDomainEventHandler<TestDomainEvent>> results = sut.ResolveEventHandlers(domainEvent);

            Assert.AreEqual(1, results.Count());
            Assert.AreSame(eventHandler, results.Single());
        }

        [TestMethod]
        public void MultipleRegisteredEventHandlersAreReturned()
        {
            IDomainEventHandler<IDomainEvent> eventHandler2 = SetupEventHandler();
            IDomainEventHandler<IDomainEvent> eventHandler1 = SetupEventHandler();

            IEnumerable<IDomainEventHandler<TestDomainEvent>> results = sut.ResolveEventHandlers(domainEvent);

            Assert.AreEqual(2, results.Count());
            Assert.IsTrue(results.Contains(eventHandler1));
            Assert.IsTrue(results.Contains(eventHandler2));
        }

        private IDomainEventHandler<IDomainEvent> SetupEventHandler()
        {
            IDomainEventHandler<IDomainEvent> eventHandler = Substitute.For<IDomainEventHandler<IDomainEvent>>();
            eventHandlers.Add(eventHandler);
            return eventHandler;
        }
    }
}