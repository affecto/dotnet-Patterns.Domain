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
            this.componentContext = componentContext ?? throw new ArgumentNullException(nameof(componentContext));
        }

        /// <summary>
        /// Resolves the set of event handlers registered for handling the given domain event. These event handlers use the Unit of Work pattern.
        /// </summary>
        /// <typeparam name="TEventHandler">The type of the domain event handler.</typeparam>
        /// <returns>A collection of event handler instances.</returns>
        public IEnumerable<TEventHandler> ResolveEventHandlers<TEventHandler>() where TEventHandler : class, IUnitOfWorkDomainEventHandler
        {
            return componentContext.Resolve<IEnumerable<TEventHandler>> ();
        }
    }
}