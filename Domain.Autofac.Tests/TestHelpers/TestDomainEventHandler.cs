namespace Affecto.Patterns.Domain.Autofac.Tests.TestHelpers
{
    public class TestDomainEventHandler : IDomainEventHandler<TestDomainEvent>
    {
        public void Execute(TestDomainEvent @event)
        {
        }
    }
}