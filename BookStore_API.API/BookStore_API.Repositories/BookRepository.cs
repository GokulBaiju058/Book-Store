

using BookStore_API.Data.Entity;
using BookStore_API.Models;
using BookStore_API.Repositories.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BookStore_API.Repositories
{
    public class BookRepository : BaseRepository<Book>, IBookRepository
    {
        public BookRepository(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, ILogger<BaseRepository<Book>> logger) : base(unitOfWork, httpContextAccessor, logger)
        {
        }

        public async Task<ResponseMessage<BookDetailViewDto>> GetBookDetails(int bookId)
        {
            var book = await _unitOfWork._context.Set<Book>().FirstOrDefaultAsync(x => x.Id == bookId);

            if (book == null)
            {
                return new ResponseMessage<BookDetailViewDto>()
                {
                    Data = null,
                    Message = $"No Book Found With Id - {bookId}",
                    Success = false
                };
            }

            var bookDetailViewDto = new BookDetailViewDto
            {
                Id = book.Id,
                BookName = book.BookName,
                Author = book.Author,
                Genre = book.Genre,
                IsActive = book.IsActive,
                CurrentQty = book.CurrentQty,
                TotalQty = book.TotalQty,
                Borrowers = new List<BorrowerDto>() // Initialize Borrowers list
            };

            // Retrieve borrowed data for the book including User details
            var borrowedData = await _unitOfWork._context.Set<BorrowedBook>()
                                            .Include(bb => bb.User) // Include User to get borrower details
                                            .Where(bb => bb.BookId == bookId && bb.ReturnedDate == null)
                                            .ToListAsync();
            if (borrowedData != null && borrowedData.Any())
            {
                foreach (var borrowedBook in borrowedData)
                {
                    if (borrowedBook.User != null) // Ensure User is not null
                    {
                        bookDetailViewDto.Borrowers.Add(new BorrowerDto
                        {
                            Username = borrowedBook.User.Username,
                            BorrowedDate = borrowedBook.BorrowedDate
                        });
                    }
                }
            }

            return new ResponseMessage<BookDetailViewDto>()
            {
                Data = bookDetailViewDto,
                Success = true
            };
        }

    }
}
