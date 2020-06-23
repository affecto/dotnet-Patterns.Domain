using System;
using System.Collections.Generic;
using System.Linq;

namespace Affecto.Patterns.Domain.UnitOfWork
{
    /// <summary>
    /// Resolves domain event handlers that use the Unit of Work pattern.
    /// </summary>
    public class UnitOfWorkDomainEventHandlerResolver : IUnitOfWorkDomainEventHandlerResolver
    {
        private readonly IReadOnlyCollection<IUnitOfWorkDomainEventHandler> eventHandlers;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitOfWorkDomainEventHandlerResolver"/> class.
        /// </summary>
        /// <param name="eventHandlers">A collection of domain event handler instances.</param>
        public UnitOfWorkDomainEventHandlerResolver(IReadOnlyCollection<IUnitOfWorkDomainEventHandler> eventHandlers)
        {
            this.eventHandlers = eventHandlers ?? throw new ArgumentNullException(nameof(eventHandlers));
        }

        /// <summary>
        /// Resolves the set of event handlers registered for handling the given domain event. These event handlers use the Unit of Work pattern.
        /// </summary>
        /// <typeparam name="TEventHandler">The type of the domain event handler.</typeparam>
        /// <returns>A collection of event handler instances.</returns>
        public IReadOnlyCollection<TEventHandler> ResolveEventHandlers<TEventHandler>()
            where TEventHandler : class, IUnitOfWorkDomainEventHandler
        {
            return eventHandlers.OfType<TEventHandler>().ToList();
        }
    }
}