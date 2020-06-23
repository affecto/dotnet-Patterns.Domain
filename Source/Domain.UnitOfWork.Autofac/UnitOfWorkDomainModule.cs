using Affecto.Patterns.Domain.Autofac;
using Autofac;

namespace Affecto.Patterns.Domain.UnitOfWork.Autofac
{
    public class UnitOfWorkDomainModule : DomainModule
    {
        /// <summary>
        /// Adds registrations to the container.
        /// </summary>
        /// <param name="builder">The builder through which components can be registered.</param>
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<ContainerUnitOfWorkDomainEventHandlerResolver>().As<IUnitOfWorkDomainEventHandlerResolver>();
        }
    }
}