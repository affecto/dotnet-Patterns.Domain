using System;
using System.Collections.Generic;
using Autofac;

namespace Affecto.Patterns.Domain.UnitOfWork.Autofac
{
    /// <summary>
    /// Resolves event handlers for domain events from Autofac component context.
    /// </summary>
    public class ContainerUnitOfWorkDomainEventHandlerResolver : IUnitOfWorkDomainEventHandlerResolver
    {
        private readonly IComponentContext componentContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContainerUnitOfWorkDomainEventHandlerResolver"/> class.
        /// </summary>
        /// <param name="componentContext">Autofac component context.</param>
        public ContainerUnitOfWorkDomainEventHandlerResolver(IComponentContext componentContext)
        {
            if (componentContext == null)
            {
                throw new ArgumentNullException("componentContext");
            }

            this.componentContext = componentContext;
        }

        /// <summary>
        /// Resolves the set of event handlers registered for handling the given domain event. These event handlers use the Unit of Work pattern.
        /// </summary>
        /// <typeparam name="TEvent">The type of the domain event.</typeparam>
        /// <typeparam name="TUnitOfWork">The type of the Unit of Work context.</typeparam>
        /// <param name="domainEvent">The domain event instance.</param>
        /// <returns>A collection of event handler instances.</returns>
        public IEnumerable<IUnitOfWorkDomainEventHandler<TEvent, TUnitOfWork>> Resolve<TEvent, TUnitOfWork>(TEvent domainEvent)
            where TEvent : class, IDomainEvent
            where TUnitOfWork : class, IUnitOfWork
        {
            return componentContext.Resolve<IEnumerable<IUnitOfWorkDomainEventHandler<TEvent, TUnitOfWork>>>();
        }
    }
}