using System.Threading.Tasks;

namespace Affecto.Patterns.Domain
{
    /// <summary>
    /// Handler for asynchronously executing domain events applied to an aggregate root or other domain entity instance.
    /// </summary>
    /// <typeparam name="TEvent">The type of the applied domain event.</typeparam>
    public interface IAsyncDomainEventHandler<in TEvent> : IDomainEventHandler
        where TEvent : class, IDomainEvent
    {
        /// <summary>
        /// Executes the given domain event asynchronously.
        /// </summary>
        /// <param name="event">The domain event instance.</param>
        Task ExecuteAsync(TEvent @event);
    }
}