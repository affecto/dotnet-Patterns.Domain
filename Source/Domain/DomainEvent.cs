using System;

namespace Affecto.Patterns.Domain
{
    /// <summary>
    /// Base class for implementing domain events that represent changes in a domain entity.
    /// </summary>
    public abstract class DomainEvent : IDomainEvent
    {
        /// <summary>
        /// Gets the entity id.
        /// </summary>
        public Guid EntityId { get; }

        /// <summary>
        /// Gets the entity version.
        /// </summary>
        public long EntityVersion { get; internal set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DomainEvent"/> class.
        /// </summary>
        /// <param name="entityId">Entity instance id.</param>
        /// <param name="entityVersion">Entity instance version.</param>
        protected DomainEvent(Guid entityId, long entityVersion)
        {
            if (entityId.Equals(Guid.Empty))
            {
                throw new ArgumentException("Entity id must be defined.", nameof(entityId));
            }

            EntityId = entityId;
            EntityVersion = entityVersion;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DomainEvent"/> class.
        /// </summary>
        /// <param name="entityId">Entity instance id.</param>
        protected DomainEvent(Guid entityId)
            : this(entityId, 0)
        {
        }
    }
}