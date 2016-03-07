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
        /// Publishes an event to all event handlers that are registered to it.
        /// </summary>
        /// <param name="event">Domain event.</param>
        public void PublishEvent(IDomainEvent @event)
        {
            if (@event == null)
            {
                throw new ArgumentNullException("event");
            }

            PublishEvent((dynamic) @event);
        }

        /// <summary>
        /// Publishes an event asynchronously to all event handlers that are registered to it.
        /// </summary>
        /// <param name="event">Domain event.</param>
        public async Task PublishEventAsync(IDomainEvent @event)
        {
            if (@event == null)
            {
                throw new ArgumentNullException("event");
            }

            await PublishEvent((dynamic) @event);
        }

        /// <summary>
        /// Publishes events to all event handlers that are registered to the event.
        /// </summary>
        /// <param name="events">Domain events.</param>
        public void PublishEvents(IEnumerable<IDomainEvent> events)
        {
            if (events == null)
            {
                throw new ArgumentNullException("events");
            }

            foreach (IDomainEvent domainEvent in events)
            {
                if (domainEvent == null)
                {
                    throw new ArgumentNullException("events", "Event list cannot contain null events.");
                }

                PublishEvent((dynamic) domainEvent);
            }
        }

        /// <summary>
        /// Publishes events asynchronously to all event handlers that are registered to the event.
        /// </summary>
        /// <param name="events">Domain events.</param>
        public async Task PublishEventsAsync(IEnumerable<IDomainEvent> events)
        {
            if (events == null)
            {
                throw new ArgumentNullException("events");
            }

            foreach (IDomainEvent domainEvent in events)
            {
                if (domainEvent == null)
                {
                    throw new ArgumentNullException("events", "Event list cannot contain null events.");
                }

                await PublishEventAsync((dynamic) domainEvent);
            }
        }

        /// <summary>
        /// Publishes the given domain event to all registered event handlers for the event type.
        /// </summary>
        /// <typeparam name="TDomainEvent">The type of the domain event.</typeparam>
        /// <param name="domainEvent">The domain event instance to execute.</param>
        protected abstract void PublishEvent<TDomainEvent>(TDomainEvent domainEvent) where TDomainEvent : class, IDomainEvent;

        /// <summary>
        /// Publishes the given domain event asynchronously to all registered event handlers for the event type.
        /// </summary>
        /// <typeparam name="TDomainEvent">The type of the domain event.</typeparam>
        /// <param name="domainEvent">The domain event instance to execute.</param>
        protected abstract Task PublishEventAsync<TDomainEvent>(TDomainEvent domainEvent) where TDomainEvent : class, IDomainEvent;
    }
}