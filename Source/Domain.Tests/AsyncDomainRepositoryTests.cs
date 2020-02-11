using System;
using System.Collections.Generic;
using Affecto.Patterns.Domain.Tests.TestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Affecto.Patterns.Domain.Tests
{
    [TestClass]
    public class AsyncDomainRepositoryTests
    {
        private IAsyncDomainEventHandler<TestDomainEvent> domainEventHandler1;
        private IAsyncDomainEventHandler<TestDomainEvent> domainEventHandler2;
        private IDomainEventHandlerResolver eventHandlerResolver;
        private TestDomainEvent domainEvent;

        private TestAsyncDomainRepository sut;

        [TestInitialize]
        public void Setup()
        {
            domainEventHandler1 = Substitute.For<IAsyncDomainEventHandler<TestDomainEvent>>();
            domainEventHandler2 = Substitute.For<IAsyncDomainEventHandler<TestDomainEvent>>();

            IEnumerable<IAsyncDomainEventHandler<TestDomainEvent>> eventHandlers = new List<IAsyncDomainEventHandler<TestDomainEvent>>
            {
                domainEventHandler1,
                domainEventHandler2
            };

            domainEvent = new TestDomainEvent(Guid.NewGuid());
            eventHandlerResolver = Substitute.For<IDomainEventHandlerResolver>();
            eventHandlerResolver.ResolveEventHandlers<IAsyncDomainEventHandler<TestDomainEvent>>().Returns(eventHandlers);

            sut = new TestAsyncDomainRepository(eventHandlerResolver);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullEventHandlerResolverThrowsException()
        {
            sut = new TestAsyncDomainRepository(null);
        }

        [TestMethod]
        public void ResolvedEventHandlersAreExecutedInCorrectOrder()
        {
            TestAggregateRoot aggregateRoot = new TestAggregateRoot(Guid.NewGuid());
            aggregateRoot.ApplyEvent(domainEvent);

            sut.ApplyChangesAsync(aggregateRoot).Wait();

            Received.InOrder(() =>
            {
                domainEventHandler1.ExecuteAsync(domainEvent);
                domainEventHandler2.ExecuteAsync(domainEvent);
            });
        }

        [TestMethod]
        public void InaccessibleTypeParameterCanBeUsed()
        {
            // This test is necessary for checking that the repository can be used with inaccessible type parameters.
            // See the bug in .NET Framework: https://connect.microsoft.com/VisualStudio/feedback/details/672411/bug-in-dynamic-dispatch-when-using-generic-type-parameters

            InternalAggregateRoot aggregateRoot = new InternalAggregateRoot(Guid.NewGuid());
            aggregateRoot.ApplyEvent(domainEvent);

            var privateSut = new InternalAsyncDomainRepository(eventHandlerResolver);
            privateSut.ApplyChangesAsync(aggregateRoot).Wait();

            domainEventHandler1.Received().ExecuteAsync(domainEvent);
            domainEventHandler2.Received().ExecuteAsync(domainEvent);
        }
    }
}