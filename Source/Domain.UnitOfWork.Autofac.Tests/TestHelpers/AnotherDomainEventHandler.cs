using System.Threading.Tasks;

namespace Affecto.Patterns.Domain.UnitOfWork.Autofac.Tests.TestHelpers
{
    public class AnotherDomainEventHandler : IUnitOfWorkDomainEventHandler<TestDomainEvent, TestUnitOfWork>
    {
        public void Execute(TestDomainEvent @event, TestUnitOfWork unitOfWork)
        {
        }

        public Task ExecuteAsync(TestDomainEvent @event, TestUnitOfWork unitOfWork)
        {
            return Task.Run(() => Execute(@event, unitOfWork));
        }
    }
}