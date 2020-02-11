using System;

namespace Affecto.Patterns.Domain.Tests.TestHelpers
{
    public class TestDomainEvent : DomainEvent
    {
        public TestDomainEvent(Guid entityId, long entityVersion)
            : base(entityId, entityVersion)
        {
        }

        public TestDomainEvent(Guid entityId)
            : base(entityId)
        {
        }
    }
}