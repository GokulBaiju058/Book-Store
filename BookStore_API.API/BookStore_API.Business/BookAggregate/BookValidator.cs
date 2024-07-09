
using BookStore_API.Models;
using FluentValidation;

namespace BookStore_API.Business.BookAggregate
{
    public class BookValidator : AbstractValidator<BookDto>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BookValidator"/> class.
        /// Configures validation rules for the Books properties.
        /// </summary>
        public BookValidator()
        {
            RuleFor(x => x.BookName).NotEmpty().WithMessage("The {PropertyName} is required.");
            RuleFor(x => x.isActive).NotEmpty().WithMessage("The {PropertyName} is required.");
            RuleFor(x => x.TotalQty).NotEmpty().WithMessage("The {PropertyName} is required.");
            RuleFor(x => x.CurrentQty).NotEmpty().WithMessage("The {PropertyName} is required.");
        }
    }

    public class BorrowBookValidator : AbstractValidator<BorrowBookDto>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BookValidator"/> class.
        /// Configures validation rules for the Books properties.
        /// </summary>
        public BorrowBookValidator()
        {
            RuleFor(x => x.BookId).NotEmpty().WithMessage("The {PropertyName} is required.");
            RuleFor(x => x.UserId).NotEmpty().WithMessage("The {PropertyName} is required.");
        }
    }
}
