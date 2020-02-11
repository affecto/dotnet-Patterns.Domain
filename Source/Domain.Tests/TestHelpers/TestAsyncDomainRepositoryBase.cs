using System;
using System.Threading.Tasks;

namespace Affecto.Patterns.Domain.Tests.TestHelpers
{
    public class TestAsyncDomainRepositoryBase : AsyncDomainRepositoryBase<TestAggregateRoot>
    {
        public TestAsyncDomainRepositoryBase()
            : base(new TestDomainEventBroker())
        {
        }

        public TestDomainEventBroker EventBroker
        {
            get { return (TestDomainEventBroker) domainEventBroker; }
        }

        public override Task<TestAggregateRoot> FindAsync(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}