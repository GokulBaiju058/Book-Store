using BookStore_API.API.Extensions;
using BookStore_API.Business.Abstractions;
using BookStore_API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookStore_API.API.Controllers
{
    /// <summary>
    /// Controller responsible for handling user-specific operations.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "User")] // Requires authorization with 'User' role for all actions in this controller
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger; // Logger for logging information
        private readonly IUserService _userService; // Service for user-related operations
        private readonly IBookService _bookService; // Service for book-related operations

        /// <summary>
        /// Initializes a new instance of the <see cref="UserController"/> class.
        /// </summary>
        /// <param name="logger">Logger instance for logging information related to user operations.</param>
        /// <param name="userService">Service interface providing methods for user management.</param>
        /// <param name="bookService">Service interface providing methods for book-related operations.</param>
        public UserController(ILogger<UserController> logger, IUserService userService, IBookService bookService)
        {
            _logger = logger;
            _userService = userService;
            _bookService = bookService;
        }

        /// <summary>
        /// Endpoint for retrieving user details by user ID.
        /// </summary>
        /// <param name="userId">The ID of the user to retrieve.</param>
        /// <returns>Response containing the user's information.</returns>
        [HttpGet]
        [Route("GetUserById")]
        public async Task<ResponseMessage<UserDto>> GetById(int userId)
        {
            _logger.LogInformation($"UserController - Get Single User for UserId - {userId}");
            return await _userService.GetAsync(userId); // Delegate user retrieval to service
        }

        /// <summary>
        /// Endpoint for updating user details.
        /// </summary>
        /// <param name="userDto">DTO containing updated user information.</param>
        /// <returns>Response containing the updated user's information.</returns>
        [HttpPut]
        [Route("UpdateUser")]
        public async Task<ResponseMessage<UserDto>> Update(RegisterUserDto userDto)
        {
            _logger.LogInformation("UserController - Update User."); // Log user update attempt
            return await _userService.UpdateAsync(userDto); // Delegate user update to service
        }

        /// <summary>
        /// Endpoint for retrieving books borrowed by the current user.
        /// </summary>
        /// <returns>Response containing a list of borrowed books by the current user.</returns>
        [HttpPost]
        [Route("GetMyBorrowedBooks")]
        public ResponseMessage<List<BorrowedBookViewDto>> GetMyBorrowedBooks()
        {
            _logger.LogInformation("UserController - Get books Borrowed By the Current User.");
            int userId = HttpContext.User.GetUserId(); // Get current user's ID from HttpContext
            return _bookService.GetAllBorrowedBookByUser(userId); // Delegate to service to get borrowed books
        }
    }
}
