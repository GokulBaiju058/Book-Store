using BookStore_API.Data;
using BookStore_API.Repositories.Abstractions;

namespace BookStore_API.Repositories
{
    /// <summary>
    /// Unit of Work pattern implementation for managing database transactions.
    /// </summary>
    public class UnitOfWork : IDisposable, IUnitOfWork
    {
        /// <summary>
        /// The database context managed by this unit of work.
        /// </summary>
        public BookStore_APIContext _context { get; private set; }
        private bool _disposed = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitOfWork"/> class with the provided database context.
        /// </summary>
        public UnitOfWork(BookStore_APIContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Disposes the current instance of the unit of work and releases its resources.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            _disposed = true;
        }

        /// <summary>
        /// Disposes the current instance of the unit of work and releases its resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Commits all changes made in the current unit of work to the underlying database.
        /// </summary>
        public async Task CommitAsync()
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }
    }
}
