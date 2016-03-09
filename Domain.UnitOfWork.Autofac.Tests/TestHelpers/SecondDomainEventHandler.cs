using System.Threading.Tasks;

namespace Affecto.Patterns.Domain.UnitOfWork.Autofac.Tests.TestHelpers
{
    public class SecondDomainEventHandler : IUnitOfWorkDomainEventHandler<SecondTestDomainEvent, TestUnitOfWork>
    {
        public void Execute(SecondTestDomainEvent @event, TestUnitOfWork unitOfWork)
        {
        }

        public Task ExecuteAsync(SecondTestDomainEvent @event, TestUnitOfWork unitOfWork)
        {
            return Task.Run(() => Execute(@event, unitOfWork));
        }
    }
}