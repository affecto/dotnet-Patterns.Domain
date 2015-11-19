using System.Collections.Generic;

namespace Affecto.Patterns.Domain
{
    /// <summary>
    /// Base class for brokers publishing domain events to domain event handlers.
    /// </summary>
    public abstract class DomainEventBrokerBase
    {
        /// <summary>
        /// Publishes events to all event handlers that are registered to the event.
        /// </summary>
        /// <param name="events">Domain events.</param>
        public void PublishEvents(IEnumerable<IDomainEvent> events)
        {
            foreach (IDomainEvent domainEvent in events)
            {
                PublishEvent((dynamic) domainEvent);
            }
        }

        /// <summary>
        /// Publishes the given domain event to all registered event handlers for the event type.
        /// </summary>
        /// <typeparam name="TDomainEvent">The type of the domain event.</typeparam>
        /// <param name="domainEvent">The domain event instance to execute.</param>
        protected abstract void PublishEvent<TDomainEvent>(TDomainEvent domainEvent) where TDomainEvent : class, IDomainEvent;
    }
}