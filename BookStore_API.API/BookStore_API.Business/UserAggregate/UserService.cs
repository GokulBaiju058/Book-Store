using BookStore_API.Business.Abstractions;
using BookStore_API.Business.Extensions;
using BookStore_API.Data.Entity;
using BookStore_API.Models;
using BookStore_API.Business.Services.PasswordHasher;
using Mapster;
using BookStore_API.Repositories.Abstractions;
using System;
using System.Threading.Tasks;
using BookStore_API.Data;

namespace BookStore_API.Business.UserAggregate
{
    /// <summary>
    /// Service for managing user operations such as registration, retrieval, update, and deletion.
    /// </summary>
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserService"/> class.
        /// </summary>
        /// <param name="userRepository">The repository for accessing user data.</param>
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        /// <summary>
        /// Registers a new user asynchronously.
        /// </summary>
        /// <param name="registerUserDto">The DTO containing user registration information.</param>
        /// <returns>A response containing the newly registered user DTO.</returns>
        public async Task<ResponseMessage<UserDto>> AddAsync(RegisterUserDto registerUserDto)
        {
            new ValidatorExtensions.GenericValidation<RegisterUserDto, UserValidator>().Validate(registerUserDto);

            // Adapt DTO to entity
            User saveUser = registerUserDto.Adapt<User>();

            // Setting user role as User (assuming RoleId 1 represents 'User' role)
            saveUser.RoleId = 1;

            // Hashing password
            saveUser.Password = PasswordHasher.Hash(registerUserDto.Password);

            // Making user active
            saveUser.isActive = true;

            // Inserting user into repository
            var newUser = await _userRepository.InsertAsync(saveUser);

            return new ResponseMessage<UserDto>() { Data = newUser.Adapt<UserDto>() };
        }

        /// <summary>
        /// Deletes a user asynchronously based on the provided ID.
        /// </summary>
        /// <param name="id">The ID of the user to delete.</param>
        /// <returns>A response indicating the success of the deletion operation.</returns>
        public Task<ResponseMessage<bool>> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Retrieves a paged list of users based on optional parameters.
        /// </summary>
        /// <param name="pageNumber">The page number of the results.</param>
        /// <param name="pageSize">The size of each page.</param>
        /// <param name="orderBy">The field to order results by.</param>
        /// <param name="orderDirection">The direction of ordering (ascending or descending).</param>
        /// <param name="search">A search keyword to filter results.</param>
        /// <returns>A response containing the paged list of user DTOs.</returns>
        public ResponseMessage<PagedList<UserDto>> GetAll(int? pageNumber, int? pageSize, string orderBy, bool orderDirection, string search)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Retrieves a user asynchronously based on the provided ID.
        /// </summary>
        /// <param name="id">The ID of the user to retrieve.</param>
        /// <returns>A response containing the user DTO.</returns>
        public Task<ResponseMessage<UserDto>> GetAsync(int id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Updates a user asynchronously.
        /// </summary>
        /// <param name="t">The DTO containing updated user information.</param>
        /// <returns>A response containing the updated user DTO.</returns>
        public Task<ResponseMessage<UserDto>> UpdateAsync(UserDto t)
        {
            throw new NotImplementedException();
        }
    }
}
