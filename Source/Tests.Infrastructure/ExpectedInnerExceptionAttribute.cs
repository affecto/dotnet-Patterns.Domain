using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Affecto.Patterns.Domain.Tests.Infrastructure
{
    /// <summary>
    /// Indicates that an exception with inner exception is expected during test method execution.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class ExpectedInnerExceptionAttribute : ExpectedExceptionBaseAttribute
    {
        public Type ExceptionType { get; }
        public Type InnerExceptionType { get; }

        /// <param name="exceptionType">An expected type of exception to be thrown by a method.</param>
        /// <param name="innerExceptionType">An expected type of inner exception to be thrown by a method.</param>
        public ExpectedInnerExceptionAttribute(Type exceptionType, Type innerExceptionType)
        {
            if (exceptionType == null)
            {
                throw new ArgumentNullException(nameof(exceptionType));
            }
            if (innerExceptionType == null)
            {
                throw new ArgumentNullException(nameof(innerExceptionType));
            }
            if (!typeof(Exception).IsAssignableFrom(exceptionType))
            {
                throw new ArgumentException("Expected exception type must derive from exception.", nameof(exceptionType));
            }
            if (!typeof(Exception).IsAssignableFrom(innerExceptionType))
            {
                throw new ArgumentException("Expected inner exception type must derive from exception.", nameof(innerExceptionType));
            }

            ExceptionType = exceptionType;
            InnerExceptionType = innerExceptionType;
        }

        protected override void Verify(Exception exception)
        {
            Type type = exception.GetType();
            if (type != ExceptionType)
            {
                RethrowIfAssertException(exception);
                throw new Exception($"Expected exception type '{ExceptionType.FullName}' but type '{type.FullName}' was thrown.");
            }

            if (exception.InnerException == null)
            {
                throw new Exception($"Expected inner exception type '{InnerExceptionType.FullName}' but no inner exception was thrown.");
            }

            type = exception.InnerException.GetType();
            if (type != InnerExceptionType)
            {
                RethrowIfAssertException(exception);
                throw new Exception($"Expected inner exception type '{InnerExceptionType.FullName}' but type '{type.FullName}' was thrown.");
            }
        }
    }
}