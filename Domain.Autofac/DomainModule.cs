using Autofac;

namespace Affecto.Patterns.Domain.Autofac
{
    public class DomainModule : Module
    {
        /// <summary>
        /// Adds registrations to the container.
        /// </summary>
        /// <param name="builder">The builder through which components can be registered.</param>
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<ContainerDomainEventHandlerResolver>().As<IDomainEventHandlerResolver>();
        }
    }
}