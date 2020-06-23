using System.Threading.Tasks;

namespace Affecto.Patterns.Domain
{
    /// <summary>
    /// Base class for a synchronous domain event handler. Executes domain events applied to an aggregate root or other domain entity instance.
    /// </summary>
    /// <typeparam name="TEvent">The type of the applied domain event.</typeparam>
    public abstract class DomainEventHandler<TEvent> : IDomainEventHandler<TEvent>
        where TEvent : class, IDomainEvent
    {
        public Task ExecuteAsync(TEvent @event)
        {
            Execute(@event);
            return Task.CompletedTask;
        }

        public abstract void Execute(TEvent @event);
    }
}