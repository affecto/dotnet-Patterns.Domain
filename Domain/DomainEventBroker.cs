using System;
using System.Collections.Generic;

namespace Affecto.Patterns.Domain
{
    /// <summary>
    /// A broker that publishes domain events to domain event handlers.
    /// </summary>
    public class DomainEventBroker : DomainEventBrokerBase
    {
        private readonly IDomainEventHandlerResolver eventHandlerResolver;

        /// <summary>
        /// Initializes a new instance of the <see cref="DomainEventBroker"/> class.
        /// </summary>
        /// <param name="eventHandlerResolver">Event handler resolver instance for finding domain event handlers.</param>
        public DomainEventBroker(IDomainEventHandlerResolver eventHandlerResolver)
        {
            if (eventHandlerResolver == null)
            {
                throw new ArgumentNullException("eventHandlerResolver");
            }

            this.eventHandlerResolver = eventHandlerResolver;
        }

        /// <summary>
        /// Publishes the given domain event to all registered event handlers for the event type.
        /// </summary>
        /// <typeparam name="TDomainEvent">The type of the domain event.</typeparam>
        /// <param name="domainEvent">The domain event instance to execute.</param>
        protected override void PublishEvent<TDomainEvent>(TDomainEvent domainEvent)
        {
            IEnumerable<IDomainEventHandler<TDomainEvent>> eventHandlers = eventHandlerResolver.Resolve(domainEvent);

            foreach (IDomainEventHandler<TDomainEvent> eventHandler in eventHandlers)
            {
                eventHandler.Execute(domainEvent);
            }
        }
    }
}