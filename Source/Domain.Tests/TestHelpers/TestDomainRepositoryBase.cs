using System;

namespace Affecto.Patterns.Domain.Tests.TestHelpers
{
    public class TestDomainRepositoryBase : DomainRepositoryBase<TestAggregateRoot>
    {
        public TestDomainRepositoryBase()
            : base(new TestDomainEventBroker())
        {
        }

        public TestDomainEventBroker EventBroker => (TestDomainEventBroker) domainEventBroker;

        public override TestAggregateRoot Find(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}