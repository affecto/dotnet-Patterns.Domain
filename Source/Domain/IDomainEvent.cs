using System;

namespace Affecto.Patterns.Domain
{
    /// <summary>
    /// Domain event that represents changes in a domain entity.
    /// </summary>
    public interface IDomainEvent
    {
        /// <summary>
        /// Gets the entity id.
        /// </summary>
        Guid EntityId { get; }

        /// <summary>
        /// Gets the entity version.
        /// </summary>
        long EntityVersion { get; }
    }
}