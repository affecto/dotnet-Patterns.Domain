using System;

namespace Affecto.Patterns.Domain.Tests.TestHelpers
{
    public class TestAggregateRoot : AggregateRoot
    {
        public TestAggregateRoot(Guid id, long version = 0)
            : base(id, version)
        {
        }

        public new void AddEvent(DomainEvent @event)
        {
            base.AddEvent(@event);
        }
    }
}