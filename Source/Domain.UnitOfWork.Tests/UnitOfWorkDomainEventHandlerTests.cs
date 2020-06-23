using System;
using System.Threading.Tasks;
using Affecto.Patterns.Domain.Tests.TestHelpers;
using Affecto.Patterns.Domain.UnitOfWork.Tests.TestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Affecto.Patterns.Domain.UnitOfWork.Tests
{
    [TestClass]
    public class UnitOfWorkDomainEventHandlerTests
    {
        private UnitOfWorkDomainEventHandler<TestDomainEvent, TestUnitOfWork> sut;

        [TestInitialize]
        public void Setup()
        {
            sut = Substitute.For<UnitOfWorkDomainEventHandler<TestDomainEvent, TestUnitOfWork>>();
        }

        [TestMethod]
        public async Task SynchronousMethodIsCalledFromAsynchronousVersion()
        {
            var domainEvent = new TestDomainEvent(Guid.NewGuid());
            var unitOfWork = new TestUnitOfWork();

            await sut.ExecuteAsync(domainEvent, unitOfWork);

            sut.Received().Execute(domainEvent, unitOfWork);
        }
    }
}