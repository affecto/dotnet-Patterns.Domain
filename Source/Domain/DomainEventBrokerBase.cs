using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Affecto.Patterns.Domain
{
    /// <summary>
    /// Base class for brokers publishing domain events to domain event handlers.
    /// </summary>
    public abstract class DomainEventBrokerBase : IDomainEventBroker
    {
        /// <summary>
        /// Publishes an event asynchronously to all event handlers that are registered to it.
        /// </summary>
        /// <param name="event">Domain event.</param>
        public async Task PublishEventAsync(IDomainEvent @event)
        {
            if (@event == null)
            {
                throw new ArgumentNullException(nameof(@event));
            }

            await PublishAsync((dynamic) @event).ConfigureAwait(false);
        }

        /// <summary>
        /// Publishes events asynchronously to all event handlers that are registered to the event.
        /// </summary>
        /// <param name="events">Domain events.</param>
        public async Task PublishEventsAsync(IEnumerable<IDomainEvent> events)
        {
            if (events == null)
            {
                throw new ArgumentNullException(nameof(events));
            }

            foreach (IDomainEvent domainEvent in events)
            {
                if (domainEvent == null)
                {
                    throw new ArgumentNullException(nameof(events), "Event list cannot contain null events.");
                }

                await PublishAsync((dynamic) domainEvent).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Publishes the given domain event asynchronously to all registered event handlers for the event type.
        /// </summary>
        /// <typeparam name="TDomainEvent">The type of the domain event.</typeparam>
        /// <param name="domainEvent">The domain event instance to execute.</param>
        protected abstract Task PublishAsync<TDomainEvent>(TDomainEvent domainEvent) where TDomainEvent : class, IDomainEvent;
    }
}