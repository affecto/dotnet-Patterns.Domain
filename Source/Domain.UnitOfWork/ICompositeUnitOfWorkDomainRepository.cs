using System;

namespace Affecto.Patterns.Domain.UnitOfWork
{
    /// <summary>
    /// Composite domain repository for given aggregate root types.
    /// </summary>
    /// <typeparam name="TAggregateRoot1">The first aggregate root type.</typeparam>
    /// <typeparam name="TAggregateRoot2">The second aggregate root type.</typeparam>
    public interface ICompositeUnitOfWorkDomainRepository<TAggregateRoot1, TAggregateRoot2>
        where TAggregateRoot1 : AggregateRoot
        where TAggregateRoot2 : AggregateRoot
    {
        /// <summary>
        /// Finds an aggregate root instance from the repository using its id.
        /// </summary>
        /// <param name="id">Aggregate root instance id.</param>
        /// <returns>Aggregate root instance.</returns>
        TAggregateRoot Find<TAggregateRoot>(Guid id) where TAggregateRoot : AggregateRoot;

        /// <summary>
        /// Executes all events that have been applied to the given aggregate root instance.
        /// </summary>
        /// <param name="aggregateWithFirstProcessedEvents">The changed aggregate root instance whose domain events will be executed first.</param>
        /// <param name="aggregateWithSecondProcessedEvents">The changed aggregate root instance whose domain events will be executed second.</param>
        void ApplyChanges(TAggregateRoot1 aggregateWithFirstProcessedEvents, TAggregateRoot2 aggregateWithSecondProcessedEvents);

        /// <summary>
        /// Executes all events that have been applied to the given aggregate root instance.
        /// </summary>
        /// <param name="aggregateWithFirstProcessedEvents">The changed aggregate root instance whose domain events will be executed first.</param>
        /// <param name="aggregateWithSecondProcessedEvents">The changed aggregate root instance whose domain events will be executed second.</param>
        void ApplyChanges(TAggregateRoot2 aggregateWithFirstProcessedEvents, TAggregateRoot1 aggregateWithSecondProcessedEvents);
    }
}