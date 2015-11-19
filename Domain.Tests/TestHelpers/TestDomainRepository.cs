using System;

namespace Affecto.Patterns.Domain.Tests.TestHelpers
{
    public class TestDomainRepository : DomainRepository<TestAggregateRoot>
    {
        public TestDomainRepository(IDomainEventHandlerResolver eventHandlerResolver)
            : base(eventHandlerResolver)
        {
        }

        public override TestAggregateRoot Find(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}