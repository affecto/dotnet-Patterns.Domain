using System;
using System.Collections.Generic;
using Affecto.Patterns.Domain.Tests.TestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Affecto.Patterns.Domain.Tests
{
    [TestClass]
    public class DomainRepositoryTests
    {
        private IDomainEventHandler<TestDomainEvent> domainEventHandler1;
        private IDomainEventHandler<TestDomainEvent> domainEventHandler2;
        private IDomainEventHandlerResolver eventHandlerResolver;
        private TestDomainEvent domainEvent;

        private TestDomainRepository sut;

        [TestInitialize]
        public void Setup()
        {
            domainEventHandler1 = Substitute.For<IDomainEventHandler<TestDomainEvent>>();
            domainEventHandler2 = Substitute.For<IDomainEventHandler<TestDomainEvent>>();

            IEnumerable<IDomainEventHandler<TestDomainEvent>> eventHandlers = new List<IDomainEventHandler<TestDomainEvent>>
            {
                domainEventHandler1,
                domainEventHandler2
            };

            domainEvent = new TestDomainEvent(Guid.NewGuid());
            eventHandlerResolver = Substitute.For<IDomainEventHandlerResolver>();
            eventHandlerResolver.ResolveEventHandlers(domainEvent).Returns(eventHandlers);

            sut = new TestDomainRepository(eventHandlerResolver);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullEventHandlerResolverThrowsException()
        {
            sut = new TestDomainRepository(null);
        }

        [TestMethod]
        public void ResolvedEventHandlersAreExecutedInCorrectOrder()
        {
            TestAggregateRoot aggregateRoot = new TestAggregateRoot(Guid.NewGuid());
            aggregateRoot.ApplyEvent(domainEvent);

            sut.ApplyChanges(aggregateRoot);

            Received.InOrder(() =>
            {
                domainEventHandler1.Execute(domainEvent);
                domainEventHandler2.Execute(domainEvent);
            });
        }

        [TestMethod]
        public void InaccessibleTypeParameterCanBeUsed()
        {
            // This test is necessary for checking that the repository can be used with inaccessible type parameters.
            // See the bug in .NET Framework: https://connect.microsoft.com/VisualStudio/feedback/details/672411/bug-in-dynamic-dispatch-when-using-generic-type-parameters

            InternalAggregateRoot aggregateRoot = new InternalAggregateRoot(Guid.NewGuid());
            aggregateRoot.ApplyEvent(domainEvent);

            var privateSut = new InternalDomainRepository(eventHandlerResolver);
            privateSut.ApplyChanges(aggregateRoot);

            domainEventHandler1.Received().Execute(domainEvent);
            domainEventHandler2.Received().Execute(domainEvent);
        }
    }
}