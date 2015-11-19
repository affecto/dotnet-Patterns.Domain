using System;
using Affecto.Patterns.Domain.Tests.TestHelpers;

namespace Affecto.Patterns.Domain.UnitOfWork.Tests.TestHelpers
{
    internal class InternalCompositeRepository : CompositeUnitOfWorkDomainRepository<InternalUnitOfWork, InternalAggregateRoot, TestAggregateRoot2>
    {
        public InternalCompositeRepository(IDomainEventHandlerResolver eventHandlerResolver, IUnitOfWorkDomainEventHandlerResolver unitOfWorkEventHandlerResolver, InternalUnitOfWork unitOfWork)
            : base(eventHandlerResolver, unitOfWorkEventHandlerResolver, unitOfWork)
        {
        }

        protected override InternalAggregateRoot FindAggregateRootOfFirstSpecifiedType(Guid id)
        {
            throw new NotImplementedException();
        }

        protected override TestAggregateRoot2 FindAggregateRootOfSecondSpecifiedType(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}