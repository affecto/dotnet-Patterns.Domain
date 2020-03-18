using System.Threading.Tasks;

namespace Affecto.Patterns.Domain.UnitOfWork
{
    /// <summary>
    /// Base class for an asynchronous domain event handler. Executes domain events applied to an aggregate root or other domain entity instance using the Unit of Work pattern.
    /// </summary>
    /// <typeparam name="TEvent">The type of the applied domain event.</typeparam>
    /// <typeparam name="TUnitOfWork">The type of the Unit of Work context.</typeparam>
    public abstract class AsyncUnitOfWorkDomainEventHandler<TEvent, TUnitOfWork> : IUnitOfWorkDomainEventHandler<TEvent, TUnitOfWork>
        where TEvent : class, IDomainEvent
        where TUnitOfWork : class, IUnitOfWork
    {
        async Task IUnitOfWorkDomainEventHandler<TEvent, TUnitOfWork>.ExecuteAsync(TEvent @event, TUnitOfWork unitOfWork)
        {
            await ExecuteAsync(@event, unitOfWork).ConfigureAwait(false);
        }

        public abstract Task ExecuteAsync(TEvent @event, TUnitOfWork unitOfWork);
    }
}