using System.Threading.Tasks;

namespace Affecto.Patterns.Domain.UnitOfWork
{
    /// <summary>
    /// Handler for asynchronously executing domain events applied to an aggregate root or other domain entity instance. The handler uses the Unit of Work pattern.
    /// </summary>
    /// <typeparam name="TEvent">The type of the applied domain event.</typeparam>
    /// <typeparam name="TUnitOfWork">The type of the Unit of Work context.</typeparam>
    public interface IAsyncUnitOfWorkDomainEventHandler<in TEvent, in TUnitOfWork> : IUnitOfWorkDomainEventHandler
        where TEvent : class, IDomainEvent
        where TUnitOfWork : class, IUnitOfWork
    {
        /// <summary>
        /// Executes the given domain event asynchronously.
        /// </summary>
        /// <param name="event">The domain event instance.</param>
        /// <param name="unitOfWork">The Unit of Work context instance.</param>
        Task ExecuteAsync(TEvent @event, TUnitOfWork unitOfWork);
    }
}