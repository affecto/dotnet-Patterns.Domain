using System;
using System.Collections.Generic;

namespace Affecto.Patterns.Domain
{
    /// <summary>
    /// Base class for implementing aggregate root domain objects.
    /// </summary>
    public abstract class AggregateRoot
    {
        private readonly List<DomainEvent> appliedEvents;

        /// <summary>
        /// Gets the aggregate root instance id.
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// Gets the aggregate root instance version.
        /// </summary>
        public long Version { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateRoot"/> class.
        /// </summary>
        /// <param name="id">Aggregate root instance id.</param>
        /// <param name="version">Aggregate root instance version.</param>
        protected AggregateRoot(Guid id, long version)
        {
            if (id == default(Guid))
            {
                throw new ArgumentException("Id must be defined.", nameof(id));
            }

            Id = id;
            Version = version;
            appliedEvents = new List<DomainEvent>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateRoot"/> class.
        /// </summary>
        /// <param name="id">Aggregate root instance id.</param>
        protected AggregateRoot(Guid id)
            : this(id, 0)
        {
        }

        /// <summary>
        /// Gets all domain events that have been applied to the aggregate root instance.
        /// </summary>
        /// <returns>A collection of domain events.</returns>
        public virtual IEnumerable<IDomainEvent> GetAppliedEvents()
        {
            return appliedEvents;
        }

        /// <summary>
        /// Sets the version of the aggregate root. The version is passed to all generated domain events.
        /// </summary>
        /// <param name="version">Aggregate root version.</param>
        public void SetVersion(long version)
        {
            Version = version;

            foreach (DomainEvent @event in appliedEvents)
            {
                @event.EntityVersion = version;
            }
        }
        
        /// <summary>
        /// Applies a new domain event to the aggregate root instance.
        /// </summary>
        /// <param name="domainEvent">The new domain event to apply.</param>
        protected void ApplyEvent(DomainEvent domainEvent)
        {
            domainEvent.EntityVersion = Version;
            appliedEvents.Add(domainEvent);
        }
    }
}