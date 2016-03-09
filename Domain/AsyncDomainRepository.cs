namespace Affecto.Patterns.Domain
{
    /// <summary>
    /// Base class for implementing asynchronous domain repositories for aggregate root types.
    /// </summary>
    /// <typeparam name="TAggregate">The type of the aggregate root.</typeparam>
    public abstract class AsyncDomainRepository<TAggregate> : AsyncDomainRepositoryBase<TAggregate>
        where TAggregate : AggregateRoot
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DomainRepository{TAggregate}"/> class.
        /// </summary>
        /// <param name="eventHandlerResolver">Event handler resolver for finding domain event handlers.</param>
        protected AsyncDomainRepository(IDomainEventHandlerResolver eventHandlerResolver)
            : base(new DomainEventBroker(eventHandlerResolver))
        {
        }
    }
}