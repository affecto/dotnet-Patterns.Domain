using System;

namespace Affecto.Patterns.Domain.UnitOfWork.Tests.TestHelpers
{
    public class TestAggregateRoot2 : AggregateRoot
    {
        public TestAggregateRoot2(Guid id)
            : base(id)
        {
        }

        public new void AddEvent(DomainEvent @event)
        {
            base.AddEvent(@event);
        }
    }
}