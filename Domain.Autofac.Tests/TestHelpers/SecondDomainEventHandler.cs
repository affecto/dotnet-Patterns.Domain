namespace Affecto.Patterns.Domain.Autofac.Tests.TestHelpers
{
    public class SecondDomainEventHandler : IDomainEventHandler<SecondTestDomainEvent>
    {
        public void Execute(SecondTestDomainEvent @event)
        {
        }
    }
}