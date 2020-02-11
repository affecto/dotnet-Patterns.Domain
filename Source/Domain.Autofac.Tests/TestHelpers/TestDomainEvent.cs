using System;

namespace Affecto.Patterns.Domain.Autofac.Tests.TestHelpers
{
    public class TestDomainEvent : IDomainEvent
    {
        public Guid EntityId { get; set; }
        public long EntityVersion { get; set; }
    }
}