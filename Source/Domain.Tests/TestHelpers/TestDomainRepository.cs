using System;
using System.Threading.Tasks;

namespace Affecto.Patterns.Domain.Tests.TestHelpers
{
    public class TestDomainRepository : DomainRepository<TestAggregateRoot>
    {
        public TestDomainRepository(IDomainEventHandlerResolver eventHandlerResolver)
            : base(eventHandlerResolver)
        {
        }

        public override Task<TestAggregateRoot> FindAsync(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}