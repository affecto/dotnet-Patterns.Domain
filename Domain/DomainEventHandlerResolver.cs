using System;
using System.Collections.Generic;
using System.Linq;

namespace Affecto.Patterns.Domain
{
    /// <summary>
    /// Resolves event handlers for domain events.
    /// </summary>
    public class DomainEventHandlerResolver : IDomainEventHandlerResolver
    {
        protected readonly IEnumerable<IDomainEventHandler> eventHandlers;

        /// <summary>
        /// Initializes a new instance of the <see cref="DomainEventHandlerResolver"/> class.
        /// </summary>
        /// <param name="eventHandlers">A collection of domain event handler instances.</param>
        public DomainEventHandlerResolver(IEnumerable<IDomainEventHandler> eventHandlers)
        {
            if (eventHandlers == null)
            {
                throw new ArgumentNullException("eventHandlers");
            }

            this.eventHandlers = eventHandlers;
        }

        /// <summary>
        /// Resolves the set of event handlers registered for handling the given domain event.
        /// </summary>
        /// <typeparam name="TEvent">The type of the domain event.</typeparam>
        /// <param name="domainEvent">The domain event instance.</param>
        /// <returns>A collection of event handler instances.</returns>
        public virtual IEnumerable<IDomainEventHandler<TEvent>> Resolve<TEvent>(TEvent domainEvent)
            where TEvent : class, IDomainEvent
        {
            return eventHandlers.OfType<IDomainEventHandler<TEvent>>();
        }
    }
}