using System.Threading.Tasks;

namespace Affecto.Patterns.Domain.UnitOfWork
{
    /// <summary>
    /// Base class for a synchronous domain event handler. Executes domain events applied to an aggregate root or other domain entity instance using the Unit of Work pattern.
    /// </summary>
    /// <typeparam name="TEvent">The type of the applied domain event.</typeparam>
    /// <typeparam name="TUnitOfWork">The type of the Unit of Work context.</typeparam>
    public abstract class UnitOfWorkDomainEventHandler<TEvent, TUnitOfWork> : IUnitOfWorkDomainEventHandler<TEvent, TUnitOfWork>
        where TEvent : class, IDomainEvent
        where TUnitOfWork : class, IUnitOfWork
    {
        Task IUnitOfWorkDomainEventHandler<TEvent, TUnitOfWork>.ExecuteAsync(TEvent @event, TUnitOfWork unitOfWork)
        {
            Execute(@event, unitOfWork);
            return Task.CompletedTask;
        }

        public abstract void Execute(TEvent @event, TUnitOfWork unitOfWork);
    }
}