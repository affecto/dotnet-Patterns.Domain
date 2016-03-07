using System.Collections.Generic;
using System.Threading.Tasks;

namespace Affecto.Patterns.Domain
{
    /// <summary>
    /// A broker that publishes domain events to domain event handlers.
    /// </summary>
    public interface IDomainEventBroker
    {
        /// <summary>
        /// Publishes an event to all event handlers that are registered to it.
        /// </summary>
        /// <param name="event">Domain event.</param>
        void PublishEvent(IDomainEvent @event);

        /// <summary>
        /// Publishes an event asynchronously to all event handlers that are registered to it.
        /// </summary>
        /// <param name="event">Domain event.</param>
        Task PublishEventAsync(IDomainEvent @event);

        /// <summary>
        /// Publishes events to all event handlers that are registered to the event.
        /// </summary>
        /// <param name="events">Domain events.</param>
        void PublishEvents(IEnumerable<IDomainEvent> events);

        /// <summary>
        /// Publishes events asynchronously to all event handlers that are registered to the event.
        /// </summary>
        /// <param name="events">Domain events.</param>
        Task PublishEventsAsync(IEnumerable<IDomainEvent> events);
    }
}