// ReSharper disable ObjectCreationAsStatement
// ReSharper disable PossibleMultipleEnumeration

using System;
using System.Collections.Generic;
using System.Linq;
using Affecto.Patterns.Domain.Autofac.Tests.TestHelpers;
using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Affecto.Patterns.Domain.Autofac.Tests
{
    [TestClass]
    public class ContainerDomainEventHandlerResolverTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ContainerCannotBeNull()
        {
            new ContainerDomainEventHandlerResolver(null);
        }

        [TestMethod]
        public void EmptyListIsReturnedIfNotHandlersRegistered()
        {
            var builder = new ContainerBuilder();
            IContainer container = builder.Build();

            ContainerDomainEventHandlerResolver sut = new ContainerDomainEventHandlerResolver(container);
            IEnumerable<IDomainEventHandler<TestDomainEvent>> results = sut.Resolve(new TestDomainEvent());

            Assert.AreEqual(0, results.Count());
        }

        [TestMethod]
        public void SingleRegisteredEventHandlerIsReturned()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<TestDomainEventHandler>().As<IDomainEventHandler<TestDomainEvent>>();
            builder.RegisterType<SecondDomainEventHandler>().As<IDomainEventHandler<SecondTestDomainEvent>>();
            IContainer container = builder.Build();

            ContainerDomainEventHandlerResolver sut = new ContainerDomainEventHandlerResolver(container);
            IEnumerable<IDomainEventHandler<TestDomainEvent>> results = sut.Resolve(new TestDomainEvent());

            Assert.AreEqual(1, results.Count());
            Assert.IsInstanceOfType(results.Single(), typeof(TestDomainEventHandler));
        }

        [TestMethod]
        public void MultipleRegisteredEventHandlersAreReturned()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<TestDomainEventHandler>().As<IDomainEventHandler<TestDomainEvent>>();
            builder.RegisterType<SecondDomainEventHandler>().As<IDomainEventHandler<SecondTestDomainEvent>>();
            builder.RegisterType<AnotherDomainEventHandler>().As<IDomainEventHandler<TestDomainEvent>>();
            IContainer container = builder.Build();

            ContainerDomainEventHandlerResolver sut = new ContainerDomainEventHandlerResolver(container);
            IEnumerable<IDomainEventHandler<TestDomainEvent>> results = sut.Resolve(new TestDomainEvent());

            Assert.AreEqual(2, results.Count());

            Assert.IsNotNull(results.OfType<TestDomainEventHandler>().SingleOrDefault());
            Assert.IsNotNull(results.OfType<AnotherDomainEventHandler>().SingleOrDefault());
        }
    }
}