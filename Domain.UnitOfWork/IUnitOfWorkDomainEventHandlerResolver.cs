using System.Collections.Generic;

namespace Affecto.Patterns.Domain.UnitOfWork
{
    /// <summary>
    /// Resolves domain event handlers that use the Unit of Work pattern.
    /// </summary>
    public interface IUnitOfWorkDomainEventHandlerResolver
    {
        /// <summary>
        /// Resolves the set of event handlers registered for handling the given domain event. These event handlers use the Unit of Work pattern.
        /// </summary>
        /// <typeparam name="TEventHandler">The type of the domain event handler.</typeparam>
        /// <returns>A collection of event handler instances.</returns>
        IEnumerable<TEventHandler> ResolveEventHandlers<TEventHandler>()
            where TEventHandler : class, IUnitOfWorkDomainEventHandler;
    }
}