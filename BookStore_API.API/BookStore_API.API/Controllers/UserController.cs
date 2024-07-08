using BookStore_API.Business.Abstractions;
using BookStore_API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookStore_API.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize] // Requires authorization for all actions in this controller
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger; // Logger for logging information
        private readonly IUserService _userService; // Service for user-related operations

        public UserController(ILogger<UserController> logger, IUserService userService)
        {
            _logger = logger;
            _userService = userService;
        }

        /// <summary>
        /// Endpoint for registering a new user.
        /// </summary>
        /// <param name="registerUserDto">DTO containing user registration information.</param>
        /// <returns>Response containing information about the registered user.</returns>
        [HttpPost]
        [Route("Register")]
        public async Task<ResponseMessage<UserDto>> Add(RegisterUserDto registerUserDto)
        {
            _logger.LogInformation("UserController - Create New User."); // Log user registration attempt
            return await _userService.AddAsync(registerUserDto); // Delegate user registration to service
        }
    }
}
