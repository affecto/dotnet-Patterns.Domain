using System;

namespace Affecto.Patterns.Domain.UnitOfWork
{
    /// <summary>
    /// Base class for implementing composite domain repositories using the Unit of Work pattern for two aggregate root types.
    /// </summary>
    /// <typeparam name="TUnitOfWork">The type of the Unit of Work context.</typeparam>
    /// <typeparam name="TAggregateRoot1">The type of the first aggregate root.</typeparam>
    /// <typeparam name="TAggregateRoot2">The type of the second aggregate root.</typeparam>
    public abstract class CompositeUnitOfWorkDomainRepository<TUnitOfWork, TAggregateRoot1, TAggregateRoot2>
        : ICompositeUnitOfWorkDomainRepository<TAggregateRoot1, TAggregateRoot2>
        where TUnitOfWork : class, IUnitOfWork
        where TAggregateRoot1 : AggregateRoot
        where TAggregateRoot2 : AggregateRoot
    {
        protected readonly TUnitOfWork unitOfWork;
        protected readonly DomainEventBroker immediateEventBroker;
        protected readonly UnitOfWorkDomainEventBroker<TUnitOfWork> unitOfWorkDomainEventBroker;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeUnitOfWorkDomainRepository{TUnitOfWork, TAggregateRoot1, TAggregateRoot2}"/> class.
        /// </summary>
        /// <param name="eventHandlerResolver">Event handler resolver for finding domain event handlers.</param>
        /// <param name="unitOfWorkEventHandlerResolver">Event handler resolver for finding unit-of-work domain event handlers.</param>
        /// <param name="unitOfWork">The Unit of Work context instance.</param>
        protected CompositeUnitOfWorkDomainRepository(IDomainEventHandlerResolver eventHandlerResolver, IUnitOfWorkDomainEventHandlerResolver unitOfWorkEventHandlerResolver, TUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            immediateEventBroker = new DomainEventBroker(eventHandlerResolver);
            unitOfWorkDomainEventBroker = new UnitOfWorkDomainEventBroker<TUnitOfWork>(unitOfWorkEventHandlerResolver, unitOfWork);
        }

        /// <summary>
        /// Finds an aggregate root instance from the repository using its id.
        /// </summary>
        /// <param name="id">Aggregate root instance id.</param>
        /// <returns>Aggregate root instance.</returns>
        public TAggregateRoot Find<TAggregateRoot>(Guid id) where TAggregateRoot : AggregateRoot
        {
            Type aggregateRootType = typeof(TAggregateRoot);

            if (aggregateRootType == typeof(TAggregateRoot1))
            {
                TAggregateRoot1 aggregateRoot = FindAggregateRootOfFirstSpecifiedType(id);
                return (TAggregateRoot) Convert.ChangeType(aggregateRoot, typeof(TAggregateRoot1), null);
            }
            if (aggregateRootType == typeof(TAggregateRoot2))
            {
                TAggregateRoot2 aggregateRoot = FindAggregateRootOfSecondSpecifiedType(id);
                return (TAggregateRoot) Convert.ChangeType(aggregateRoot, typeof(TAggregateRoot2), null);
            }

            throw new IncompatibleAggregateRootTypeException(
                $"Aggregate root of type '{aggregateRootType}' cannot be used through this composite domain repository.");
        }

        /// <summary>
        /// Executes all unit-of-work events that have been applied to the given aggregate root instances, then commits the unit of work.
        /// After a successful commit all other domain events are executed.
        /// </summary>
        /// <param name="aggregateWithFirstProcessedEvents">The changed aggregate root instance whose domain events will be executed first.</param>
        /// <param name="aggregateWithSecondProcessedEvents">The changed aggregate root instance whose domain events will be executed second.</param>
        public void ApplyChanges(TAggregateRoot1 aggregateWithFirstProcessedEvents, TAggregateRoot2 aggregateWithSecondProcessedEvents)
        {
            if (aggregateWithFirstProcessedEvents == null)
            {
                throw new ArgumentNullException(nameof(aggregateWithFirstProcessedEvents));
            }
            if (aggregateWithSecondProcessedEvents == null)
            {
                throw new ArgumentNullException(nameof(aggregateWithSecondProcessedEvents));
            }

            unitOfWorkDomainEventBroker.PublishEvents(aggregateWithFirstProcessedEvents.GetAppliedEvents());
            unitOfWorkDomainEventBroker.PublishEvents(aggregateWithSecondProcessedEvents.GetAppliedEvents());
            unitOfWork.SaveChanges();

            immediateEventBroker.PublishEvents(aggregateWithFirstProcessedEvents.GetAppliedEvents());
            immediateEventBroker.PublishEvents(aggregateWithSecondProcessedEvents.GetAppliedEvents());
        }

        /// <summary>
        /// Executes all unit-of-work events that have been applied to the given aggregate root instances, then commits the unit of work.
        /// After a successful commit all other domain events are executed.
        /// </summary>
        /// <param name="aggregateWithFirstProcessedEvents">The changed aggregate root instance whose domain events will be executed first.</param>
        /// <param name="aggregateWithSecondProcessedEvents">The changed aggregate root instance whose domain events will be executed second.</param>
        public void ApplyChanges(TAggregateRoot2 aggregateWithFirstProcessedEvents, TAggregateRoot1 aggregateWithSecondProcessedEvents)
        {
            if (aggregateWithFirstProcessedEvents == null)
            {
                throw new ArgumentNullException(nameof(aggregateWithFirstProcessedEvents));
            }
            if (aggregateWithSecondProcessedEvents == null)
            {
                throw new ArgumentNullException(nameof(aggregateWithSecondProcessedEvents));
            }

            unitOfWorkDomainEventBroker.PublishEvents(aggregateWithFirstProcessedEvents.GetAppliedEvents());
            unitOfWorkDomainEventBroker.PublishEvents(aggregateWithSecondProcessedEvents.GetAppliedEvents());
            unitOfWork.SaveChanges();

            immediateEventBroker.PublishEvents(aggregateWithFirstProcessedEvents.GetAppliedEvents());
            immediateEventBroker.PublishEvents(aggregateWithSecondProcessedEvents.GetAppliedEvents());
        }

        /// <summary>
        /// Finds an aggregate root instance from the repository using its id.
        /// </summary>
        /// <param name="id">Aggregate root instance id.</param>
        /// <returns>Aggregate root instance.</returns>
        protected abstract TAggregateRoot1 FindAggregateRootOfFirstSpecifiedType(Guid id);

        /// <summary>
        /// Finds an aggregate root instance from the repository using its id.
        /// </summary>
        /// <param name="id">Aggregate root instance id.</param>
        /// <returns>Aggregate root instance.</returns>
        protected abstract TAggregateRoot2 FindAggregateRootOfSecondSpecifiedType(Guid id);
    }
}