// ReSharper disable ObjectCreationAsStatement

using System;
using Affecto.Patterns.Domain.UnitOfWork.Tests.TestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Affecto.Patterns.Domain.UnitOfWork.Tests
{
    [TestClass]
    public class UnitOfWorkDomainEventBrokerTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void EventHandlerResolverCannotBeNull()
        {
            new UnitOfWorkDomainEventBroker<TestUnitOfWork>(null, new TestUnitOfWork());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void UnitOfWorkCannotBeNull()
        {
            new UnitOfWorkDomainEventBroker<TestUnitOfWork>(Substitute.For<IUnitOfWorkDomainEventHandlerResolver>(), null);
        }
    }
}