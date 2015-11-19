using System.Collections.Generic;

namespace Affecto.Patterns.Domain
{
    /// <summary>
    /// Resolves event handlers for domain events.
    /// </summary>
    public interface IDomainEventHandlerResolver
    {
        /// <summary>
        /// Resolves the set of event handlers registered for handling the given domain event.
        /// </summary>
        /// <typeparam name="TEvent">The type of the domain event.</typeparam>
        /// <param name="domainEvent">The domain event instance.</param>
        /// <returns>A collection of event handler instances.</returns>
        IEnumerable<IDomainEventHandler<TEvent>> Resolve<TEvent>(TEvent domainEvent)
            where TEvent : class, IDomainEvent;
    }
}