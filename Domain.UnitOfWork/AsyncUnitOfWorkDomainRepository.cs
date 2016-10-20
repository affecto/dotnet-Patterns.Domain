using System;
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
    public abstract class AsyncUnitOfWorkDomainRepository<TUnitOfWork, TAggregate> : AsyncDomainRepositoryBase<TAggregate>
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
        protected AsyncUnitOfWorkDomainRepository(IDomainEventHandlerResolver eventHandlerResolver,
            IUnitOfWorkDomainEventHandlerResolver unitOfWorkEventHandlerResolver, TUnitOfWork unitOfWork)
            : base(new UnitOfWorkDomainEventBroker<TUnitOfWork>(unitOfWorkEventHandlerResolver, unitOfWork))
        {
            if (unitOfWork == null)
            {
                throw new ArgumentNullException("unitOfWork");
            }

            this.unitOfWork = unitOfWork;
            immediateEventBroker = new DomainEventBroker(eventHandlerResolver);
        }

        /// <summary>
        /// Asynchronously executes all unit-of-work events that have been applied to the given aggregate root instance, then commits the unit of work.
        /// After a succesfull commit all other domain events are executed.
        /// </summary>
        /// <param name="aggregateRoot">The changed aggregate root instance.</param>
        public override async Task ApplyChangesAsync(TAggregate aggregateRoot)
        {
            await base.ApplyChangesAsync(aggregateRoot).ConfigureAwait(false);
            unitOfWork.SaveChanges();

            await ExecuteAppliedEventsAsync(aggregateRoot, immediateEventBroker).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously executes all unit-of-work events that have been applied to the given set of aggregate root instances, then commits the unit of work.
        /// After a succesfull commit all other domain events are executed.
        /// </summary>
        /// <param name="aggregateRoots">The changed aggregate root instances.</param>
        public override async Task ApplyChangesAsync(IEnumerable<TAggregate> aggregateRoots)
        {
            IList<TAggregate> aggregates = aggregateRoots as IList<TAggregate> ?? aggregateRoots.ToList();

            await base.ApplyChangesAsync(aggregates).ConfigureAwait(false);
            unitOfWork.SaveChanges();

            foreach (TAggregate aggregateRoot in aggregates)
            {
                await ExecuteAppliedEventsAsync(aggregateRoot, immediateEventBroker).ConfigureAwait(false);
            }
        }
    }
}