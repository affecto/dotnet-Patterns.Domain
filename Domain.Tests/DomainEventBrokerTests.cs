// ReSharper disable ObjectCreationAsStatement

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Affecto.Patterns.Domain.Tests
{
    [TestClass]
    public class DomainEventBrokerTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void EventHandlerResolverCannotBeNull()
        {
            new DomainEventBroker(null);
        }
    }
}