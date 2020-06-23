using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
            this.eventHandlerResolver = eventHandlerResolver ?? throw new ArgumentNullException(nameof(eventHandlerResolver));
            this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        /// <summary>
        /// Publishes the given domain event to all registered event handlers for the event type.
        /// </summary>
        /// <typeparam name="TDomainEvent">The type of the domain event.</typeparam>
        /// <param name="domainEvent">The domain event instance to execute.</param>
        protected override async Task PublishAsync<TDomainEvent>(TDomainEvent domainEvent)
        {
            IReadOnlyCollection<IUnitOfWorkDomainEventHandler<TDomainEvent, TUnitOfWork>> eventHandlers = eventHandlerResolver.ResolveEventHandlers<IUnitOfWorkDomainEventHandler<TDomainEvent, TUnitOfWork>>();

            foreach (IUnitOfWorkDomainEventHandler<TDomainEvent, TUnitOfWork> eventHandler in eventHandlers)
            {
                await eventHandler.ExecuteAsync(domainEvent, unitOfWork).ConfigureAwait(false);
            }
        }
    }
}