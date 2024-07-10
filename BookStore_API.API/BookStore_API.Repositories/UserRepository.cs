using BookStore_API.Data.Entity;
using BookStore_API.Repositories.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BookStore_API.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, ILogger<BaseRepository<User>> logger) : base(unitOfWork, httpContextAccessor, logger)
        {
        }
    }
}
