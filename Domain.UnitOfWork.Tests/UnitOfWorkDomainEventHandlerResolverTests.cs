// ReSharper disable ObjectCreationAsStatement
// ReSharper disable PossibleMultipleEnumeration

using System;
using System.Collections.Generic;
using System.Linq;
using Affecto.Patterns.Domain.Tests.TestHelpers;
using Affecto.Patterns.Domain.UnitOfWork.Tests.TestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Affecto.Patterns.Domain.UnitOfWork.Tests
{
    [TestClass]
    public class UnitOfWorkDomainEventHandlerResolverTests
    {
        private TestDomainEvent domainEvent;
        private List<IUnitOfWorkDomainEventHandler> eventHandlers;
        private UnitOfWorkDomainEventHandlerResolver sut;

        [TestInitialize]
        public void Setup()
        {
            domainEvent = new TestDomainEvent(Guid.NewGuid());
            eventHandlers = new List<IUnitOfWorkDomainEventHandler>();

            sut = new UnitOfWorkDomainEventHandlerResolver(eventHandlers);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void EventHandlersCannotBeNull()
        {
            new UnitOfWorkDomainEventHandlerResolver(null);
        }

        [TestMethod]
        public void SingleRegisteredEventHandlerIsReturned()
        {
            IUnitOfWorkDomainEventHandler<TestDomainEvent, TestUnitOfWork> eventHandler = SetupEventHandler();

            IEnumerable<IUnitOfWorkDomainEventHandler<TestDomainEvent, TestUnitOfWork>> results = sut.ResolveEventHandlers<TestDomainEvent, TestUnitOfWork>(domainEvent);

            Assert.AreEqual(1, results.Count());
            Assert.AreSame(eventHandler, results.Single());
        }

        [TestMethod]
        public void MultipleRegisteredEventHandlersAreReturned()
        {
            IUnitOfWorkDomainEventHandler<TestDomainEvent, TestUnitOfWork> eventHandler1 = SetupEventHandler();
            IUnitOfWorkDomainEventHandler<TestDomainEvent, TestUnitOfWork> eventHandler2 = SetupEventHandler();

            IEnumerable<IUnitOfWorkDomainEventHandler<TestDomainEvent, TestUnitOfWork>> results = sut.ResolveEventHandlers<TestDomainEvent, TestUnitOfWork>(domainEvent);

            Assert.AreEqual(2, results.Count());
            Assert.IsTrue(results.Contains(eventHandler1));
            Assert.IsTrue(results.Contains(eventHandler2));
        }

        private IUnitOfWorkDomainEventHandler<TestDomainEvent, TestUnitOfWork> SetupEventHandler()
        {
            IUnitOfWorkDomainEventHandler<TestDomainEvent, TestUnitOfWork> eventHandler = Substitute.For<IUnitOfWorkDomainEventHandler<TestDomainEvent, TestUnitOfWork>>();
            eventHandlers.Add(eventHandler);
            return eventHandler;
        }
    }
}