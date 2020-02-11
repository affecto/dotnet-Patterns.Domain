using System;
using System.Collections.Generic;
using System.Linq;

namespace Affecto.Patterns.Domain
{
    /// <summary>
    /// Base class for implementing aggregate root domain objects.
    /// </summary>
    public abstract class AggregateRoot
    {
        private readonly List<DomainEvent> pendingEvents;

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
            if (id == default)
            {
                throw new ArgumentException("Id must be defined.", nameof(id));
            }

            Id = id;
            Version = version;
            pendingEvents = new List<DomainEvent>();
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
        /// Returns all domain events that are pending for publishing and dequeues them.
        /// </summary>
        /// <returns>A collection of domain events.</returns>
        public IReadOnlyCollection<IDomainEvent> DequeuePendingEvents()
        {
            List<DomainEvent> events = pendingEvents.ToList();

            pendingEvents.Clear();
            return events;
        }

        /// <summary>
        /// Sets the version of the aggregate root. The version is passed to all generated domain events.
        /// </summary>
        /// <param name="version">Aggregate root version.</param>
        public void SetVersion(long version)
        {
            Version = version;

            foreach (DomainEvent @event in pendingEvents)
            {
                @event.EntityVersion = version;
            }
        }
        
        /// <summary>
        /// Adds a new event to the list of pending events for this aggregate root.
        /// </summary>
        /// <param name="domainEvent">The new domain event to add.</param>
        protected void AddEvent(DomainEvent domainEvent)
        {
            if (domainEvent == null)
            {
                throw new ArgumentNullException(nameof(domainEvent));
            }

            domainEvent.EntityVersion = Version;
            pendingEvents.Add(domainEvent);
        }
    }
}