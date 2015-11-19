namespace Affecto.Patterns.Domain.UnitOfWork
{
    /// <summary>
    /// Represents a context instance for the Unit of Work pattern.
    /// </summary>
    public interface IUnitOfWork
    {
        /// <summary>
        /// Save all the applied changes in the Unit of Work context.
        /// </summary>
        void SaveChanges();
    }
}