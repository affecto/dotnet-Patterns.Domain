using System;
using System.Threading.Tasks;

namespace Affecto.Patterns.Domain.Tests.TestHelpers
{
    internal class InternalDomainRepository : DomainRepository<InternalAggregateRoot>
    {
        public InternalDomainRepository(IDomainEventHandlerResolver eventHandlerResolver)
            : base(eventHandlerResolver)
        {
        }

        public override Task<InternalAggregateRoot> FindAsync(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}