namespace Affecto.Patterns.Domain.UnitOfWork.Autofac.Tests.TestHelpers
{
    public class TestDomainEventHandler : IUnitOfWorkDomainEventHandler<TestDomainEvent, TestUnitOfWork>
    {
        public void Execute(TestDomainEvent @event, TestUnitOfWork unitOfWork)
        {
        }
    }
}