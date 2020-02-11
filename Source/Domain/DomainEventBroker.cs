using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
            this.eventHandlerResolver = eventHandlerResolver ?? throw new ArgumentNullException(nameof(eventHandlerResolver));
        }

        /// <summary>
        /// Publishes the given domain event to all registered event handlers for the event type.
        /// </summary>
        /// <typeparam name="TDomainEvent">The type of the domain event.</typeparam>
        /// <param name="domainEvent">The domain event instance to execute.</param>
        protected override void Publish<TDomainEvent>(TDomainEvent domainEvent)
        {
            IEnumerable<IDomainEventHandler<TDomainEvent>> eventHandlers =
                eventHandlerResolver.ResolveEventHandlers<IDomainEventHandler<TDomainEvent>>();

            foreach (IDomainEventHandler<TDomainEvent> eventHandler in eventHandlers)
            {
                eventHandler.Execute(domainEvent);
            }

            IEnumerable<IAsyncDomainEventHandler<TDomainEvent>> asyncEventHandlers =
                eventHandlerResolver.ResolveEventHandlers<IAsyncDomainEventHandler<TDomainEvent>>();

            foreach (IAsyncDomainEventHandler<TDomainEvent> eventHandler in asyncEventHandlers)
            {
                eventHandler.ExecuteAsync(domainEvent).Wait();
            }
        }

        /// <summary>
        /// Publishes the given domain event asynchronously to all registered event handlers for the event type.
        /// </summary>
        /// <typeparam name="TDomainEvent">The type of the domain event.</typeparam>
        /// <param name="domainEvent">The domain event instance to execute.</param>
        protected override async Task PublishAsync<TDomainEvent>(TDomainEvent domainEvent)
        {
            IEnumerable<IDomainEventHandler<TDomainEvent>> eventHandlers =
                eventHandlerResolver.ResolveEventHandlers<IDomainEventHandler<TDomainEvent>>();

            foreach (IDomainEventHandler<TDomainEvent> eventHandler in eventHandlers)
            {
                eventHandler.Execute(domainEvent);
            }

            IEnumerable<IAsyncDomainEventHandler<TDomainEvent>> asyncEventHandlers =
                eventHandlerResolver.ResolveEventHandlers<IAsyncDomainEventHandler<TDomainEvent>>();

            foreach (IAsyncDomainEventHandler<TDomainEvent> eventHandler in asyncEventHandlers)
            {
                await eventHandler.ExecuteAsync(domainEvent).ConfigureAwait(false);
            }
        }
    }
}