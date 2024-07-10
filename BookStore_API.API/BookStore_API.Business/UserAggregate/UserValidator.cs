
using BookStore_API.Models;
using FluentValidation;

namespace BookStore_API.Business.UserAggregate
{
    /// <summary>
    /// Validator for the registration DTO of a user.
    /// </summary>
    public class UserValidator : AbstractValidator<RegisterUserDto>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserValidator"/> class.
        /// Configures validation rules for the RegisterUserDto properties.
        /// </summary>
        public UserValidator()
        {
            RuleFor(x => x.Username).NotEmpty().WithMessage("The {PropertyName} is required.");
            RuleFor(x => x.Password).NotEmpty().WithMessage("The {PropertyName} is required.");
        }
    }
}
