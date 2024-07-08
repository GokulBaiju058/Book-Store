using BookStore_API.Models;
using System.Threading.Tasks;

namespace BookStore_API.Business.Abstractions
{
    /// <summary>
    /// Interface for user authentication service.
    /// </summary>
    public interface ILoginService
    {
        /// <summary>
        /// Authenticates a user based on the provided credentials.
        /// </summary>
        /// <param name="loginDto">DTO containing user login credentials.</param>
        /// <returns>A response containing information about the authenticated user.</returns>
        Task<ResponseMessage<LoggedUser>> AuthenticateUser(LoginDto loginDto);
    }
}
