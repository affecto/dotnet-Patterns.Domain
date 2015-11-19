using System;

namespace Affecto.Patterns.Domain.UnitOfWork.Tests.TestHelpers
{
    public class TestAggregateRoot3 : AggregateRoot
    {
        public TestAggregateRoot3(Guid id)
            : base(id)
        {
        }

        public new void ApplyEvent(DomainEvent @event)
        {
            base.ApplyEvent(@event);
        }
    }
}