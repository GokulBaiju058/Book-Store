
using BookStore_API.Data;
using BookStore_API.Models;

namespace BookStore_API.Business.Abstractions
{
    /// <summary>
    /// Interface for user-related operations
    /// </summary>
    public interface IUserService
    {
        Task<ResponseMessage<UserDto>> GetAsync(int id);
        Task<ResponseMessage<UserDto>> AddAsync(RegisterUserDto user);
        Task<ResponseMessage<UserDto>> UpdateAsync(RegisterUserDto user);
    }
}
