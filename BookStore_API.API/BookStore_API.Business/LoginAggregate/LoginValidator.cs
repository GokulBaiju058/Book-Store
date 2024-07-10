using BookStore_API.Models;
using FluentValidation;

namespace BookStore_API.Business.LoginAggregate
{
    public class LoginValidator:AbstractValidator<LoginDto>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LoginValidator"/> class.
        /// Configures validation rules for the Login properties.
        /// </summary>
        public LoginValidator()
        {
            RuleFor(x => x.Username).NotEmpty().WithMessage("The {PropertyName} is required.");
            RuleFor(x => x.Password).NotEmpty().WithMessage("The {PropertyName} is required.");
        }
    }
}
