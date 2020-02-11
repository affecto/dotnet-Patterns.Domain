using System;
using System.Threading.Tasks;

namespace Affecto.Patterns.Domain.Tests.TestHelpers
{
    internal class InternalAsyncDomainRepository : AsyncDomainRepository<InternalAggregateRoot>
    {
        public InternalAsyncDomainRepository(IDomainEventHandlerResolver eventHandlerResolver)
            : base(eventHandlerResolver)
        {
        }

        public override Task<InternalAggregateRoot> FindAsync(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}