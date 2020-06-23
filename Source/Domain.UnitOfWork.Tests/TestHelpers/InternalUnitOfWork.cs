using System.Threading.Tasks;

namespace Affecto.Patterns.Domain.UnitOfWork.Tests.TestHelpers
{
    internal class InternalUnitOfWork : IUnitOfWork
    {
        public virtual Task SaveChangesAsync()
        {
            return Task.CompletedTask;
        }
    }
}