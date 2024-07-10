using BookStore_API.Data;

namespace BookStore_API.Repositories.Abstractions
{
    public interface IUnitOfWork
    {
        BookStore_APIContext _context { get; }
        Task CommitAsync();
        void Dispose();
    }
}
