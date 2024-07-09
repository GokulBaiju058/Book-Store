
using BookStore_API.Data.Entity;
using BookStore_API.Repositories.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BookStore_API.Repositories
{
    public class BorrowedBookRepository : BaseRepository<BorrowedBook>, IBorrowedBookRepository
    {
        public BorrowedBookRepository(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, ILogger<BaseRepository<BorrowedBook>> logger) : base(unitOfWork, httpContextAccessor, logger)
        {
        }
    }
}
