using System;
using System.Threading.Tasks;
using Affecto.Patterns.Domain.Tests.TestHelpers;

namespace Affecto.Patterns.Domain.UnitOfWork.Tests.TestHelpers
{
    public class TestAsyncUnitOfWorkDomainRepository : AsyncUnitOfWorkDomainRepository<TestUnitOfWork, TestAggregateRoot>
    {
        public TestAsyncUnitOfWorkDomainRepository(IDomainEventHandlerResolver eventHandlerResolver, IUnitOfWorkDomainEventHandlerResolver unitOfWorkEventHandlerResolver, TestUnitOfWork unitOfWork)
            : base(eventHandlerResolver, unitOfWorkEventHandlerResolver, unitOfWork)
        {
        }

        public override Task<TestAggregateRoot> FindAsync(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}