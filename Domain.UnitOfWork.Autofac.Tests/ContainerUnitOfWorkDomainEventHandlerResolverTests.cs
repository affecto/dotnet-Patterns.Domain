// ReSharper disable ObjectCreationAsStatement
// ReSharper disable PossibleMultipleEnumeration

using System;
using System.Collections.Generic;
using System.Linq;
using Affecto.Patterns.Domain.UnitOfWork.Autofac.Tests.TestHelpers;
using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Affecto.Patterns.Domain.UnitOfWork.Autofac.Tests
{
    [TestClass]
    public class ContainerUnitOfWorkDomainEventHandlerResolverTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ContainerCannotBeNull()
        {
            new ContainerUnitOfWorkDomainEventHandlerResolver(null);
        }

        [TestMethod]
        public void EmptyListIsReturnedIfNotHandlersRegistered()
        {
            var builder = new ContainerBuilder();
            IContainer container = builder.Build();

            ContainerUnitOfWorkDomainEventHandlerResolver sut = new ContainerUnitOfWorkDomainEventHandlerResolver(container);
            IEnumerable<IUnitOfWorkDomainEventHandler<TestDomainEvent, TestUnitOfWork>> results = sut.ResolveEventHandlers<TestDomainEvent, TestUnitOfWork>(new TestDomainEvent());

            Assert.AreEqual(0, results.Count());
        }

        [TestMethod]
        public void SingleRegisteredEventHandlerIsReturned()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<TestDomainEventHandler>().As<IUnitOfWorkDomainEventHandler<TestDomainEvent, TestUnitOfWork>>();
            builder.RegisterType<SecondDomainEventHandler>().As<IUnitOfWorkDomainEventHandler<SecondTestDomainEvent, TestUnitOfWork>>();
            IContainer container = builder.Build();

            ContainerUnitOfWorkDomainEventHandlerResolver sut = new ContainerUnitOfWorkDomainEventHandlerResolver(container);
            IEnumerable<IUnitOfWorkDomainEventHandler<TestDomainEvent, TestUnitOfWork>> results = sut.ResolveEventHandlers<TestDomainEvent, TestUnitOfWork>(new TestDomainEvent());

            Assert.AreEqual(1, results.Count());
            Assert.IsInstanceOfType(results.Single(), typeof(TestDomainEventHandler));
        }

        [TestMethod]
        public void MultipleRegisteredEventHandlersAreReturned()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<TestDomainEventHandler>().As<IUnitOfWorkDomainEventHandler<TestDomainEvent, TestUnitOfWork>>();
            builder.RegisterType<SecondDomainEventHandler>().As<IUnitOfWorkDomainEventHandler<SecondTestDomainEvent, TestUnitOfWork>>();
            builder.RegisterType<AnotherDomainEventHandler>().As<IUnitOfWorkDomainEventHandler<TestDomainEvent, TestUnitOfWork>>();
            IContainer container = builder.Build();

            ContainerUnitOfWorkDomainEventHandlerResolver sut = new ContainerUnitOfWorkDomainEventHandlerResolver(container);
            IEnumerable<IUnitOfWorkDomainEventHandler<TestDomainEvent, TestUnitOfWork>> results = sut.ResolveEventHandlers<TestDomainEvent, TestUnitOfWork>(new TestDomainEvent());

            Assert.AreEqual(2, results.Count());

            Assert.IsNotNull(results.OfType<TestDomainEventHandler>().SingleOrDefault());
            Assert.IsNotNull(results.OfType<AnotherDomainEventHandler>().SingleOrDefault());
        }
    }
}