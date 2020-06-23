using System;
using System.Threading.Tasks;
using Affecto.Patterns.Domain.Tests.TestHelpers;

namespace Affecto.Patterns.Domain.UnitOfWork.Tests.TestHelpers
{
    internal class InternalCompositeRepository : CompositeUnitOfWorkDomainRepository<InternalUnitOfWork, InternalAggregateRoot, TestAggregateRoot2>
    {
        public InternalCompositeRepository(IDomainEventHandlerResolver eventHandlerResolver, IUnitOfWorkDomainEventHandlerResolver unitOfWorkEventHandlerResolver, InternalUnitOfWork unitOfWork)
            : base(eventHandlerResolver, unitOfWorkEventHandlerResolver, unitOfWork)
        {
        }

        protected override Task<InternalAggregateRoot> FindAggregateRootOfFirstSpecifiedTypeAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        protected override Task<TestAggregateRoot2> FindAggregateRootOfSecondSpecifiedTypeAsync(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}