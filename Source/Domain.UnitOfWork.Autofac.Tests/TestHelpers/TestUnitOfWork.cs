using System.Threading.Tasks;

namespace Affecto.Patterns.Domain.UnitOfWork.Autofac.Tests.TestHelpers
{
    public class TestUnitOfWork : IUnitOfWork
    {
        public Task SaveChangesAsync()
        {
            return Task.CompletedTask;
        }
    }
}