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
        public Type ExceptionType { get; private set; }
        public Type InnerExceptionType { get; private set; }

        /// <param name="exceptionType">An expected type of exception to be thrown by a method.</param>
        /// <param name="innerExceptionType">An expected type of inner exception to be thrown by a method.</param>
        public ExpectedInnerExceptionAttribute(Type exceptionType, Type innerExceptionType)
        {
            if (exceptionType == null)
            {
                throw new ArgumentNullException("exceptionType");
            }
            if (innerExceptionType == null)
            {
                throw new ArgumentNullException("innerExceptionType");
            }
            if (!typeof(Exception).IsAssignableFrom(exceptionType))
            {
                throw new ArgumentException("Expected exception type must derive from exception.", "exceptionType");
            }
            if (!typeof(Exception).IsAssignableFrom(innerExceptionType))
            {
                throw new ArgumentException("Expected inner exception type must derive from exception.", "innerExceptionType");
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
                throw new Exception(string.Format("Expected exception type '{0}' but type '{1}' was thrown.", ExceptionType.FullName, type.FullName));
            }

            if (exception.InnerException == null)
            {
                throw new Exception(string.Format("Expected inner exception type '{0}' but no inner exception was thrown.", InnerExceptionType.FullName));
            }

            type = exception.InnerException.GetType();
            if (type != InnerExceptionType)
            {
                RethrowIfAssertException(exception);
                throw new Exception(string.Format("Expected inner exception type '{0}' but type '{1}' was thrown.", InnerExceptionType.FullName, type.FullName));
            }
        }
    }
}