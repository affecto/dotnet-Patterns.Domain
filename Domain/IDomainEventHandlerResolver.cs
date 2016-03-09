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
        /// <typeparam name="TEventHandler">The type of the domain event handler.</typeparam>
        /// <returns>A collection of event handler instances.</returns>
        IEnumerable<TEventHandler> ResolveEventHandlers<TEventHandler>()
            where TEventHandler : class, IDomainEventHandler;
    }
}