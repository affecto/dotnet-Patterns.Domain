using System;
using System.Threading.Tasks;

namespace Affecto.Patterns.Domain.Tests.TestHelpers
{
    public class TestDomainRepositoryBase : DomainRepositoryBase<TestAggregateRoot>
    {
        public TestDomainRepositoryBase()
            : base(new TestDomainEventBroker())
        {
        }

        public TestDomainEventBroker EventBroker => (TestDomainEventBroker) domainEventBroker;

        public override Task<TestAggregateRoot> FindAsync(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}