using BookStore_API.Business.Abstractions;
using BookStore_API.Business.Extensions;
using BookStore_API.Models;
using BookStore_API.Repositories.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BookStore_API.Business.LoginAggregate
{
    /// <summary>
    /// Service responsible for user authentication and generating JWT tokens.
    /// </summary>
    public class LoginService : ILoginService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginService"/> class.
        /// </summary>
        /// <param name="userRepository">The repository for accessing user data.</param>
        /// <param name="configuration">The configuration containing authentication settings.</param>
        public LoginService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        /// <summary>
        /// Authenticates a user based on the provided credentials and generates a JWT token.
        /// </summary>
        /// <param name="loginDto">The DTO containing login credentials.</param>
        /// <returns>A response containing the authenticated user's information and JWT token.</returns>
        public async Task<ResponseMessage<LoggedUser>> AuthenticateUser(LoginDto loginDto)
        {
            new ValidatorExtensions.GenericValidation<LoginDto, LoginValidator>().Validate(loginDto);

            // Retrieve user from repository including related role information
            var user = _userRepository.Get(x => x.Username == loginDto.Username, includes: x => x.Role).FirstOrDefault();

            if (user == null)
            {
                return new ResponseMessage<LoggedUser>
                {
                    Data = null,
                    Success = false,
                    StatusCode = 404,
                    Message = "Invalid password."
                };
            }

            // Create claims for the JWT token
            var claims = new List<Claim>()
            {
                new Claim("userId", user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role.Name)
            };

            // Generate JWT token
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Authentication:JwtBearer:SecurityKey"]));
            var token = new JwtSecurityToken(
                issuer: _configuration["Authentication:JwtBearer:Issuer"],
                audience: _configuration["Authentication:JwtBearer:Audience"],
                expires: DateTime.Now.AddHours(18),
                claims: claims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var tokenString = jwtSecurityTokenHandler.WriteToken(token);

            // Create logged-in user object containing JWT token and user role
            LoggedUser loggedUser = new LoggedUser()
            {
                Token = tokenString,
                Role = user.Role.Name,
                UserName = user.Username
            };

            return new ResponseMessage<LoggedUser> { Data = loggedUser };
        }
    }
}
