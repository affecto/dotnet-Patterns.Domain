﻿using System.Threading.Tasks;

namespace Affecto.Patterns.Domain.Autofac.Tests.TestHelpers
{
    public class TestDomainEventHandler : IDomainEventHandler<TestDomainEvent>
    {
        public void Execute(TestDomainEvent @event)
        {
        }

        public Task ExecuteAsync(TestDomainEvent @event)
        {
            return Task.Run(() => Execute(@event));
        }
    }
}