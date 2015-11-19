namespace Affecto.Patterns.Domain.Autofac.Tests.TestHelpers
{
    public class AnotherDomainEventHandler : IDomainEventHandler<TestDomainEvent>
    {
        public void Execute(TestDomainEvent @event)
        {
        }
    }
}