using System;
using System.Collections.Generic;

namespace Affecto.Patterns.Domain.UnitOfWork
{
    /// <summary>
    /// A broker that publishes domain events to domain event handlers.
    /// </summary>
    public class UnitOfWorkDomainEventBroker<TUnitOfWork> : DomainEventBrokerBase
        where TUnitOfWork : class, IUnitOfWork
    {
        private readonly IUnitOfWorkDomainEventHandlerResolver eventHandlerResolver;
        private readonly TUnitOfWork unitOfWork;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitOfWorkDomainEventBroker{TUnitOfWork}"/> class.
        /// </summary>
        /// <param name="eventHandlerResolver">Event handler resolver instance for finding domain event handlers.</param>
        /// <param name="unitOfWork">The Unit of Work context instance.</param>
        public UnitOfWorkDomainEventBroker(IUnitOfWorkDomainEventHandlerResolver eventHandlerResolver, TUnitOfWork unitOfWork)
        {
            if (eventHandlerResolver == null)
            {
                throw new ArgumentNullException("eventHandlerResolver");
            }
            if (unitOfWork == null)
            {
                throw new ArgumentNullException("unitOfWork");
            }

            this.eventHandlerResolver = eventHandlerResolver;
            this.unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Publishes the given domain event to all registered event handlers for the event type.
        /// </summary>
        /// <typeparam name="TDomainEvent">The type of the domain event.</typeparam>
        /// <param name="domainEvent">The domain event instance to execute.</param>
        protected override void PublishEvent<TDomainEvent>(TDomainEvent domainEvent)
        {
            IEnumerable<IUnitOfWorkDomainEventHandler<TDomainEvent, TUnitOfWork>> eventHandlers =
                eventHandlerResolver.Resolve<TDomainEvent, TUnitOfWork>(domainEvent);

            foreach (IUnitOfWorkDomainEventHandler<TDomainEvent, TUnitOfWork> eventHandler in eventHandlers)
            {
                eventHandler.Execute(domainEvent, unitOfWork);
            }
        }
    }
}