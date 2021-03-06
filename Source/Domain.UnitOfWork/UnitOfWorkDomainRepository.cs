﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Affecto.Patterns.Domain.UnitOfWork
{
    /// <summary>
    /// Base class for implementing domain repositories using the Unit of Work pattern for aggregate root types.
    /// </summary>
    /// <typeparam name="TAggregate">The type of the aggregate root.</typeparam>
    /// <typeparam name="TUnitOfWork">The type of the Unit of Work context.</typeparam>
    public abstract class UnitOfWorkDomainRepository<TUnitOfWork, TAggregate> : DomainRepositoryBase<TAggregate>
        where TUnitOfWork : class, IUnitOfWork
        where TAggregate : AggregateRoot
    {
        protected readonly TUnitOfWork unitOfWork;
        protected readonly DomainEventBroker immediateEventBroker;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitOfWorkDomainRepository{TUnitOfWork, TAggregate}"/> class.
        /// </summary>
        /// <param name="eventHandlerResolver">Event handler resolver for finding domain event handlers.</param>
        /// <param name="unitOfWorkEventHandlerResolver">Event handler resolver for finding unit-of-work domain event handlers.</param>
        /// <param name="unitOfWork">The Unit of Work context instance.</param>
        protected UnitOfWorkDomainRepository(IDomainEventHandlerResolver eventHandlerResolver,
            IUnitOfWorkDomainEventHandlerResolver unitOfWorkEventHandlerResolver, TUnitOfWork unitOfWork)
            : base(new UnitOfWorkDomainEventBroker<TUnitOfWork>(unitOfWorkEventHandlerResolver, unitOfWork))
        {
            this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            immediateEventBroker = new DomainEventBroker(eventHandlerResolver);
        }

        /// <summary>
        /// Asynchronously executes all unit-of-work events that have been applied to the given aggregate root instance, then commits the unit of work.
        /// After a successful commit all other domain events are executed.
        /// </summary>
        /// <param name="aggregateRoot">The changed aggregate root instance.</param>
        public override async Task ApplyChangesAsync(TAggregate aggregateRoot)
        {
            if (aggregateRoot == null)
            {
                throw new ArgumentNullException(nameof(aggregateRoot));
            }

            IReadOnlyCollection<IDomainEvent> pendingEvents = aggregateRoot.GetPendingEvents();

            await PublishPendingEventsAsync(pendingEvents, domainEventBroker).ConfigureAwait(false);
            await unitOfWork.SaveChangesAsync().ConfigureAwait(false);

            await PublishPendingEventsAsync(pendingEvents, immediateEventBroker).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously executes all unit-of-work events that have been applied to the given set of aggregate root instances, then commits the unit of work.
        /// After a successful commit all other domain events are executed.
        /// </summary>
        /// <param name="aggregateRoots">The changed aggregate root instances.</param>
        public override async Task ApplyChangesAsync(IReadOnlyCollection<TAggregate> aggregateRoots)
        {
            if (aggregateRoots == null)
            {
                throw new ArgumentNullException(nameof(aggregateRoots));
            }

            if (aggregateRoots.Any(a => a == null))
            {
                throw new ArgumentNullException(nameof(aggregateRoots), "Aggregate root list cannot contain null values.");
            }

            List<IDomainEvent> allEvents = aggregateRoots.SelectMany(a => a.GetPendingEvents()).ToList();

            await PublishPendingEventsAsync(allEvents, domainEventBroker).ConfigureAwait(false);
            await unitOfWork.SaveChangesAsync().ConfigureAwait(false);

            await PublishPendingEventsAsync(allEvents, immediateEventBroker).ConfigureAwait(false);
        }
    }
}