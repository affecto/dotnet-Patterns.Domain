namespace Affecto.Patterns.Domain
{
    /// <summary>
    /// Handler for executing domain events applied to an aggregate root or other domain entity instance.
    /// </summary>
    public interface IDomainEventHandler
    {
    }

    /// <summary>
    /// Handler for executing domain events applied to an aggregate root or other domain entity instance.
    /// </summary>
    /// <typeparam name="TEvent">The type of the applied domain event.</typeparam>
    public interface IDomainEventHandler<in TEvent> : IDomainEventHandler
        where TEvent : class, IDomainEvent
    {
        /// <summary>
        /// Executes the given domain event.
        /// </summary>
        /// <param name="event">The domain event instance.</param>
        void Execute(TEvent @event);
    }
}