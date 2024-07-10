using BookStore_API.Business.Abstractions;
using BookStore_API.Business.Extensions;
using BookStore_API.Data.Entity;
using BookStore_API.Models;
using BookStore_API.Business.Services.PasswordHasher;
using Mapster;
using BookStore_API.Repositories.Abstractions;

namespace BookStore_API.Business.UserAggregate
{
    /// <summary>
    /// Service for managing user operations such as registration, retrieval, update, and deletion.
    /// </summary>
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        /// Initializes a new instance of the <see cref="UserService"/> class.
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        /// Registers a new user asynchronously.
        public async Task<ResponseMessage<UserDto>> AddAsync(RegisterUserDto registerUserDto)
        {
            new ValidatorExtensions.GenericValidation<RegisterUserDto, UserValidator>().Validate(registerUserDto);

            // Adapt DTO to entity
            User saveUser = registerUserDto.Adapt<User>();

            // Setting user role as User (assuming RoleId 1 represents 'User' role)
            saveUser.RoleId = 1;

            if (!string.IsNullOrEmpty(registerUserDto.Password))
            {
                // Hashing password
                saveUser.Password = PasswordHasher.Hash(registerUserDto.Password);
            }
            else
            {
                // Handle the case where the password is null or empty (throw exception, log, or set a default value)
                throw new Exception("Password cannot be null or empty.");
            }

            // Making user active
            saveUser.IsActive = true;

            //TODO implement Email Registration method

            // Inserting user into repository
            var newUser = await _userRepository.InsertAsync(saveUser);

            return new ResponseMessage<UserDto>() { Data = newUser.Adapt<UserDto>() };
        }

        /// Deletes a user asynchronously based on the provided ID
        public async Task<ResponseMessage<bool>> DeleteAsync(int id)
        {
            User user = await _userRepository.GetByIdAsync(id);

            if (user == null)
            {
                return new ResponseMessage<bool>() { Data = false, Message = "No User Found With Id -" + id.ToString(), Success = false };
            }
            await _userRepository.SoftDeleteAsync(user);
            return new ResponseMessage<bool>() { Data = true };
        }

        /// Retrieves a user asynchronously based on the provided ID
        public async Task<ResponseMessage<UserDto>> GetAsync(int id)
        {
            User user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return new ResponseMessage<UserDto>() { Data = null, Message = "No User Found With Id -" + id.ToString(), Success = false };
            }

            return new ResponseMessage<UserDto>() { Data = user.Adapt<UserDto>() };
        }

        public async Task<ResponseMessage<UserDto>> UpdateAsync(RegisterUserDto user)
        {
            new ValidatorExtensions.GenericValidation<RegisterUserDto, UserValidator>().Validate(user);
            if (user == null)
            {
                return new ResponseMessage<UserDto>() { Success = false, Message = " Invalid Object" };
            }
            var existinguser = await _userRepository.GetByIdAsync(user.Id);
            if (existinguser == null)
            {
                return new ResponseMessage<UserDto>() { Success = false, Message = "User Not Found" };
            }
            User updateUser = user.Adapt<User>();
            //Role Cannot be chnaged
            updateUser.RoleId = existinguser.RoleId;
            if (!string.IsNullOrEmpty(user.Password))
            {
                updateUser.Password = PasswordHasher.Hash(user.Password);
            }
            var updatedUser = await _userRepository.InsertOrUpdateAsync(user.Id, updateUser);
            return new ResponseMessage<UserDto> { Data = updatedUser.Adapt<UserDto>() };
        }
    }

}
