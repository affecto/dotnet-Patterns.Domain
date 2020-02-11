using System;
using System.Collections.Generic;

namespace Affecto.Patterns.Domain
{
    /// <summary>
    /// Domain repository for a given aggregate root type.
    /// </summary>
    /// <typeparam name="TAggregate">The type of the aggregate root.</typeparam>
    public interface IDomainRepository<TAggregate> where TAggregate : AggregateRoot
    {
        /// <summary>
        /// Finds an aggregate root instance from the repository using its id.
        /// </summary>
        /// <param name="id">Aggregate root instance id.</param>
        /// <returns>Aggregate root instance.</returns>
        TAggregate Find(Guid id);

        /// <summary>
        /// Executes all events that have been applied to the given aggregate root instance.
        /// </summary>
        /// <param name="aggregateRoot">The changed aggregate root instance.</param>
        void ApplyChanges(TAggregate aggregateRoot);

        /// <summary>
        /// Executes all events that have been applied to the given set of aggregate root instances.
        /// </summary>
        /// <param name="aggregateRoots">The changed aggregate root instances.</param>
        void ApplyChanges(IReadOnlyCollection<TAggregate> aggregateRoots);
    }
}