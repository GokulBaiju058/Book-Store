using BookStore_API.Business.Abstractions;
using BookStore_API.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookStore_API.API.Controllers
{
    /// <summary>
    /// Controller responsible for handling user login and registration operations.
    /// </summary>
    [Route("api/auth")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ILogger<LoginController> _logger; // Logger for logging information
        private readonly ILoginService _loginService; // Service for user authentication
        private readonly IUserService _userService; // Service for user management

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginController"/> class.
        /// </summary>
        /// <param name="logger">Logger instance for logging information related to user operations.</param>
        /// <param name="loginService">Service interface providing methods for user authentication.</param>
        /// <param name="userService">Service interface providing methods for user management.</param>
        public LoginController(ILogger<LoginController> logger, ILoginService loginService, IUserService userService)
        {
            _logger = logger;
            _loginService = loginService;
            _userService = userService;
        }

        /// <summary>
        /// Endpoint for user authentication.
        /// </summary>
        /// <param name="loginDto">DTO containing user credentials.</param>
        /// <returns>Response containing a logged-in user's information.</returns>
        [HttpPost("login")]
        public ResponseMessage<LoggedUser> Login(LoginDto loginDto)
        {
            _logger.LogInformation("LoginController - Authenticate User.");
            return _loginService.AuthenticateUser(loginDto);
        }

        /// <summary>
        /// Endpoint for registering a new user.
        /// </summary>
        /// <param name="registerUserDto">DTO containing user details for registration.</param>
        /// <returns>Response containing the newly registered user's information.</returns>
        [HttpPost("register")]
        public async Task<ResponseMessage<UserDto>> Add(RegisterUserDto registerUserDto)
        {
            _logger.LogInformation("LoginController - Register New User."); // Log user registration attempt
            return await _userService.AddAsync(registerUserDto); // Delegate user registration to service
        }

        //TODO: Implement ForgotPassword endpoint

    }
}
