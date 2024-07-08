using BookStore_API.Business.Abstractions;
using BookStore_API.Data;
using BookStore_API.Data.Entity;
using BookStore_API.Models;


namespace BookStore_API.Business.UserAggregate
{
    internal class UserService : IUserService
    {
        public Task<ResponseMessage<User>> AddAsync(RegisterUserDto registerUserDto)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseMessage<bool>> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public ResponseMessage<PagedList<User>> GetAll(int? pageNumber, int? pageSize, string orderBy, bool orderDirection, string search)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseMessage<User>> GetAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseMessage<User>> UpdateAsync(User t)
        {
            throw new NotImplementedException();
        }
    }
}
