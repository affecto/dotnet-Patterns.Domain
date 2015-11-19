namespace Affecto.Patterns.Domain.UnitOfWork.Autofac.Tests.TestHelpers
{
    public class SecondDomainEventHandler : IUnitOfWorkDomainEventHandler<SecondTestDomainEvent, TestUnitOfWork>
    {
        public void Execute(SecondTestDomainEvent @event, TestUnitOfWork unitOfWork)
        {
        }
    }
}