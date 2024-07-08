
using BookStore_API.Data.Entity;
using BookStore_API.Models;

namespace BookStore_API.Business.Abstractions
{
    /// <summary>
    /// Interface for user-related operations.
    /// Inherits from the base service interface with User entity, RegisterUserDto for addition, and User entity for view.
    /// </summary>
    public interface IUserService : IBaseService<UserDto,RegisterUserDto,UserDto>
    {
    }
}
