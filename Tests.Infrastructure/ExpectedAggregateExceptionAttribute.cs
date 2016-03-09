using System;

namespace Affecto.Patterns.Domain.Tests.Infrastructure
{
    /// <summary>
    /// Indicates that an aggregate exception with inner exception is expected during test method execution.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class ExpectedAggregateExceptionAttribute : ExpectedInnerExceptionAttribute
    {
        /// <param name="innerExceptionType">An expected type of inner exception to be thrown by a method.</param>
        public ExpectedAggregateExceptionAttribute(Type innerExceptionType)
            : base(typeof(AggregateException), innerExceptionType)
        {
        }
    }
}