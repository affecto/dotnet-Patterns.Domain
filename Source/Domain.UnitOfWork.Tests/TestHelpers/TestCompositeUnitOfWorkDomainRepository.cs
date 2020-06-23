using System;
using System.Threading.Tasks;
using Affecto.Patterns.Domain.Tests.TestHelpers;

namespace Affecto.Patterns.Domain.UnitOfWork.Tests.TestHelpers
{
    public class TestCompositeUnitOfWorkDomainRepository : CompositeUnitOfWorkDomainRepository<TestUnitOfWork, TestAggregateRoot, TestAggregateRoot2>
    {
        public TestCompositeUnitOfWorkDomainRepository(IDomainEventHandlerResolver eventHandlerResolver, IUnitOfWorkDomainEventHandlerResolver unitOfWorkEventHandlerResolver, TestUnitOfWork unitOfWork)
            : base(eventHandlerResolver, unitOfWorkEventHandlerResolver, unitOfWork)
        {
        }

        protected override Task<TestAggregateRoot> FindAggregateRootOfFirstSpecifiedTypeAsync(Guid id)
        {
            return Task.FromResult(new TestAggregateRoot(Guid.NewGuid()));
        }

        protected override Task<TestAggregateRoot2> FindAggregateRootOfSecondSpecifiedTypeAsync(Guid id)
        {
            return Task.FromResult(new TestAggregateRoot2(Guid.NewGuid()));
        }
    }
}