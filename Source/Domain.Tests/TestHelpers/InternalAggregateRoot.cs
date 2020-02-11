using System;

namespace Affecto.Patterns.Domain.Tests.TestHelpers
{
    public class InternalAggregateRoot : AggregateRoot
    {
        public InternalAggregateRoot(Guid id)
            : base(id)
        {
        }

        public new void AddEvent(DomainEvent @event)
        {
            base.AddEvent(@event);
        }
    }
}