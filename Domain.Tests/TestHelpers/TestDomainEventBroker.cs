using System.Collections.Generic;
using System.Threading.Tasks;
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

        protected override void Publish<TDomainEvent>(TDomainEvent domainEvent)
        {
            ExecutedEvents.Add(domainEvent);
        }

        protected override Task PublishAsync<TDomainEvent>(TDomainEvent domainEvent)
        {
            return Task.Run(() => ExecutedEvents.Add(domainEvent));
        }
    }
}