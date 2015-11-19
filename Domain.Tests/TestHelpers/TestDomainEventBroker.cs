using System.Collections.Generic;
using NSubstitute;

namespace Affecto.Patterns.Domain.Tests.TestHelpers
{
    public class TestDomainEventBroker : DomainEventBroker
    {
        public List<IDomainEvent> ExecutedEvents { get; private set; }

        public TestDomainEventBroker()
            : base(Substitute.For<IDomainEventHandlerResolver>())
        {
            ExecutedEvents = new List<IDomainEvent>();
        }

        protected override void PublishEvent<TDomainEvent>(TDomainEvent domainEvent)
        {
            ExecutedEvents.Add(domainEvent);
        }
    }
}