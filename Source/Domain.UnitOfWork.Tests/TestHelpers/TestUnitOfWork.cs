using System.Threading.Tasks;

namespace Affecto.Patterns.Domain.UnitOfWork.Tests.TestHelpers
{
    public class TestUnitOfWork : IUnitOfWork
    {
        public virtual Task SaveChangesAsync()
        {
            return Task.CompletedTask;
        }
    }
}