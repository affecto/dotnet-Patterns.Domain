using System;

namespace Affecto.Patterns.Domain.UnitOfWork
{
    public class IncompatibleAggregateRootTypeException : Exception
    {
        public IncompatibleAggregateRootTypeException(string message)
            : base(message)
        {
        }
    }
}