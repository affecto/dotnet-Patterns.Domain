using System;

namespace Affecto.Patterns.Domain.Tests.TestHelpers
{
    public class AnotherTestDomainEvent : DomainEvent
    {
        public AnotherTestDomainEvent(Guid entityId, long entityVersion)
            : base(entityId, entityVersion)
        {
        }

        public AnotherTestDomainEvent(Guid entityId)
            : base(entityId)
        {
        }
    }
}