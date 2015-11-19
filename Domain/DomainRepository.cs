namespace Affecto.Patterns.Domain
{
    /// <summary>
    /// Base class for implementing domain repositories for aggregate root types.
    /// </summary>
    /// <typeparam name="TAggregate">The type of the aggregate root.</typeparam>
    public abstract class DomainRepository<TAggregate> : DomainRepositoryBase<TAggregate>
        where TAggregate : AggregateRoot
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DomainRepository{TAggregate}"/> class.
        /// </summary>
        /// <param name="eventHandlerResolver">Event handler resolver for finding domain event handlers.</param>
        protected DomainRepository(IDomainEventHandlerResolver eventHandlerResolver)
            : base(new DomainEventBroker(eventHandlerResolver))
        {
        }
    }
}