using System;
using System.Collections.Generic;
using Autofac;

namespace Affecto.Patterns.Domain.Autofac
{
    /// <summary>
    /// Resolves event handlers for domain events from Autofac component context.
    /// </summary>
    public class ContainerDomainEventHandlerResolver : IDomainEventHandlerResolver
    {
        private readonly IComponentContext componentContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContainerDomainEventHandlerResolver"/> class.
        /// </summary>
        /// <param name="componentContext">Autofac component context.</param>
        public ContainerDomainEventHandlerResolver(IComponentContext componentContext)
        {
            if (componentContext == null)
            {
                throw new ArgumentNullException("componentContext");
            }

            this.componentContext = componentContext;
        }

        /// <summary>
        /// Resolves the set of event handlers registered for handling the given domain event.
        /// </summary>
        /// <typeparam name="TEventHandler">The type of the domain event handler.</typeparam>
        /// <returns>A collection of event handler instances.</returns>
        public IEnumerable<TEventHandler> ResolveEventHandlers<TEventHandler>() where TEventHandler : class, IDomainEventHandler
        {
            return componentContext.Resolve<IEnumerable<TEventHandler>>();
        }
    }
}