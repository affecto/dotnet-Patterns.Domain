using System;
using Affecto.Patterns.Domain.Tests.TestHelpers;

namespace Affecto.Patterns.Domain.UnitOfWork.Tests.TestHelpers
{
    public class TestUnitOfWorkDomainRepository : UnitOfWorkDomainRepository<TestUnitOfWork, TestAggregateRoot>
    {
        public TestUnitOfWorkDomainRepository(IDomainEventHandlerResolver eventHandlerResolver, IUnitOfWorkDomainEventHandlerResolver unitOfWorkEventHandlerResolver, TestUnitOfWork unitOfWork)
            : base(eventHandlerResolver, unitOfWorkEventHandlerResolver, unitOfWork)
        {
        }

        public override TestAggregateRoot Find(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}