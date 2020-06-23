using System.Threading.Tasks;

namespace Affecto.Patterns.Domain
{
    /// <summary>
    /// Base class for an asynchronous domain event handler. Executes domain events applied to an aggregate root or other domain entity instance.
    /// </summary>
    /// <typeparam name="TEvent">The type of the applied domain event.</typeparam>
    public abstract class AsyncDomainEventHandler<TEvent> : IDomainEventHandler<TEvent>
        where TEvent : class, IDomainEvent
    {
        public abstract Task ExecuteAsync(TEvent @event);
    }
}