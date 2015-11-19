using System;
using Affecto.Patterns.Domain.Tests.TestHelpers;

namespace Affecto.Patterns.Domain.UnitOfWork.Tests.TestHelpers
{
    public class TestCompositeUnitOfWorkDomainRepository : CompositeUnitOfWorkDomainRepository<TestUnitOfWork, TestAggregateRoot, TestAggregateRoot2>
    {
        public TestCompositeUnitOfWorkDomainRepository(IDomainEventHandlerResolver eventHandlerResolver, IUnitOfWorkDomainEventHandlerResolver unitOfWorkEventHandlerResolver, TestUnitOfWork unitOfWork)
            : base(eventHandlerResolver, unitOfWorkEventHandlerResolver, unitOfWork)
        {
        }

        protected override TestAggregateRoot FindAggregateRootOfFirstSpecifiedType(Guid id)
        {
            return new TestAggregateRoot(Guid.NewGuid());
        }

        protected override TestAggregateRoot2 FindAggregateRootOfSecondSpecifiedType(Guid id)
        {
            return new TestAggregateRoot2(Guid.NewGuid());
        }
    }
}