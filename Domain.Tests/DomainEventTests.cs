using System;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Affecto.Patterns.Domain.Tests
{
    [TestClass]
    public class DomainEventTests
    {
        private const long Version = 1;
        private readonly Guid id = Guid.NewGuid();
        private DomainEvent sut;

        [TestMethod]
        public void IdIsSet()
        {
            sut = Substitute.For<DomainEvent>(id);
            Assert.AreEqual(id, sut.EntityId);
        }

        [TestMethod]
        public void VersionIsSetToDefault()
        {
            sut = Substitute.For<DomainEvent>(id);
            Assert.AreEqual(0, sut.EntityVersion);
        }

        [TestMethod]
        public void IdIsSetFromTwoParameterConstructor()
        {
            sut = Substitute.For<DomainEvent>(id, Version);
            Assert.AreEqual(id, sut.EntityId);
        }

        [TestMethod]
        public void VersionIsSetFromTwoParameterConstructor()
        {
            sut = Substitute.For<DomainEvent>(id, Version);
            Assert.AreEqual(Version, sut.EntityVersion);
        }

        [TestMethod]
        [ExpectedException(typeof(TargetInvocationException))]
        public void IdCannotBeEmpty()
        {
            sut = Substitute.For<DomainEvent>(Guid.Empty);
        }

        [TestMethod]
        [ExpectedException(typeof(TargetInvocationException))]
        public void IdCannotBeEmptyInTwoParameterConstructor()
        {
            sut = Substitute.For<DomainEvent>(Guid.Empty, Version);
        }
    }
}