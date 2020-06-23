﻿using System.Threading.Tasks;

namespace Affecto.Patterns.Domain.Autofac.Tests.TestHelpers
{
    public class SecondDomainEventHandler : IDomainEventHandler<SecondTestDomainEvent>
    {
        public void Execute(SecondTestDomainEvent @event)
        {
        }

        public Task ExecuteAsync(SecondTestDomainEvent @event)
        {
            return Task.Run(() => Execute(@event));
        }
    }
}