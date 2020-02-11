using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Affecto.Patterns.Domain
{
    /// <summary>
    /// Asynchronous domain repository for a given aggregate root type.
    /// </summary>
    /// <typeparam name="TAggregate">The type of the aggregate root.</typeparam>
    public interface IAsyncDomainRepository<TAggregate> where TAggregate : AggregateRoot
    {
        /// <summary>
        /// Asynchronously finds an aggregate root instance from the repository using its id.
        /// </summary>
        /// <param name="id">Aggregate root instance id.</param>
        /// <returns>Aggregate root instance.</returns>
        Task<TAggregate> FindAsync(Guid id);

        /// <summary>
        /// Asynchronously executes all events that have been applied to the given aggregate root instance.
        /// </summary>
        /// <param name="aggregateRoot">The changed aggregate root instance.</param>
        Task ApplyChangesAsync(TAggregate aggregateRoot);

        /// <summary>
        /// Asynchronously executes all events that have been applied to the given set of aggregate root instances.
        /// </summary>
        /// <param name="aggregateRoots">The changed aggregate root instances.</param>
        Task ApplyChangesAsync(IReadOnlyCollection<TAggregate> aggregateRoots);
    }
}