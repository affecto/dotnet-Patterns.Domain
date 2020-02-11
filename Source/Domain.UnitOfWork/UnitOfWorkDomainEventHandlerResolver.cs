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
        private readonly IEnumerable<IUnitOfWorkDomainEventHandler> eventHandlers;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitOfWorkDomainEventHandlerResolver"/> class.
        /// </summary>
        /// <param name="eventHandlers">A collection of domain event handler instances.</param>
        public UnitOfWorkDomainEventHandlerResolver(IEnumerable<IUnitOfWorkDomainEventHandler> eventHandlers)
        {
            if (eventHandlers == null)
            {
                throw new ArgumentNullException("eventHandlers");
            }

            this.eventHandlers = eventHandlers;
        }

        /// <summary>
        /// Resolves the set of event handlers registered for handling the given domain event. These event handlers use the Unit of Work pattern.
        /// </summary>
        /// <typeparam name="TEventHandler">The type of the domain event handler.</typeparam>
        /// <returns>A collection of event handler instances.</returns>
        public IEnumerable<TEventHandler> ResolveEventHandlers<TEventHandler>()
            where TEventHandler : class, IUnitOfWorkDomainEventHandler
        {
            return eventHandlers.OfType<TEventHandler>();
        }
    }
}