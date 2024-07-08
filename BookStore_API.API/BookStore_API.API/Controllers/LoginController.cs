using BookStore_API.Business.Abstractions;
using BookStore_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace BookStore_API.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class LoginController : Controller
    {
        private readonly ILogger<LoginController> _logger; // Logger for logging information
        private readonly ILoginService _loginService; // Service for user authentication

        public LoginController(ILogger<LoginController> logger, ILoginService loginService)
        {
            _logger = logger;
            _loginService = loginService;
        }

        /// <summary>
        /// Endpoint for user authentication.
        /// </summary>
        /// <param name="loginDto">DTO containing user credentials.</param>
        /// <returns>Response containing a logged-in user's information.</returns>
        [HttpPost]
        [Route("Login")]
        public async Task<ResponseMessage<LoggedUser>> Login(LoginDto loginDto)
        {
            _logger.LogInformation("LoginController - Authenticate User."); // Log authentication attempt
            return await _loginService.AuthenticateUser(loginDto); // Delegate authentication to service
        }
    }
}
