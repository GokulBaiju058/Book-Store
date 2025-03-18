using BookStore_API.Business.Abstractions;
using BookStore_API.Data.Entity;
using BookStore_API.Data;
using BookStore_API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookStore_API.API.Controllers
{
    /// <summary>
    /// Controller for managing books by LibraryAdmin role.
    /// </summary>
    [Route("api/admin/book")]
    [ApiController]
    [Authorize(Roles = "LibraryAdmin")]
    public class AdminController : ControllerBase
    {
        private readonly IBookService _bookService;
        private readonly ILogger<AdminController> _logger;

        /// <summary>
        /// Constructor
        /// </summary>
        public AdminController(IBookService bookService, ILogger<AdminController> logger)
        {
            _bookService = bookService;
            _logger = logger;
        }

        /// <summary>
        /// Gets detailed information about a specific book
        /// </summary>
        /// <param name="bookId">ID of the book</param>
        [HttpGet("{bookId}/details")]
        public async Task<ResponseMessage<BookDetailViewDto>> GetBookDetails(int bookId)
        {
            _logger.LogInformation($"AdminBooksController - Getting detailed information for book {bookId}");
            return await _bookService.GetBookDetail(bookId);
        }

        /// <summary>
        /// Creates a new book
        /// </summary>
        /// <param name="bookDto">Book information</param>
        [HttpPost]
        public async Task<ResponseMessage<BookDto>> CreateBook([FromBody] BookDto bookDto)
        {
            _logger.LogInformation("AdminBooksController - Creating new book");
            return await _bookService.AddAsync(bookDto);
        }

        /// <summary>
        /// Updates an existing book
        /// </summary>
        /// <param name="bookId">ID of the book to update</param>
        /// <param name="bookDto">Updated book information</param>
        [HttpPut("{bookId}")]
        public async Task<ResponseMessage<BookDto>> UpdateBook(int bookId, [FromBody] BookDto bookDto)
        {
            if (bookId != bookDto.Id)
            {
                _logger.LogWarning($"BookId mismatch: Path ID {bookId} doesn't match body ID {bookDto.Id}");
                return new ResponseMessage<BookDto>
                {
                    Success = false,
                    Message = "BookId in URL must match BookId in request body",
                    Data = null
                };
            }

            _logger.LogInformation($"AdminBooksController - Updating book {bookId}");
            return await _bookService.UpdateAsync(bookDto);
        }

        /// <summary>
        /// Deletes a book
        /// </summary>
        /// <param name="bookId">ID of the book to delete</param>
        [HttpDelete("{bookId}")]
        public async Task<ResponseMessage<bool>> DeleteBook(int bookId)
        {
            _logger.LogInformation($"AdminBooksController - Deleting book {bookId}");
            return await _bookService.DeleteAsync(bookId);
        }

        /// <summary>
        /// Gets all books borrowed by a specific user
        /// </summary>
        /// <param name="userId">ID of the user</param>
        [HttpGet("borrowed/users/{userId}")]
        public ResponseMessage<List<BorrowedBookViewDto>> GetUserBorrowedBooks(int userId)
        {
            _logger.LogInformation($"AdminBooksController - Getting borrowed books for user {userId}");
            return _bookService.GetAllBorrowedBookByUser(userId);
        }

        /// <summary>
        /// Gets a paged list of all borrowed books
        /// </summary>
        [HttpGet("borrowed")]
        public ResponseMessage<PagedList<BorrowedBook>> GetAllBorrowedBooks(
            [FromQuery] int? pageNumber,
            [FromQuery] int? pageSize,
            [FromQuery] string orderBy = "Id",
            [FromQuery] bool orderDirection = true,
            [FromQuery] string search = "")
        {
            _logger.LogInformation("AdminBooksController - Getting all borrowed books");
            return _bookService.GetAllBorrowedBooks(pageNumber, pageSize, orderBy, orderDirection, search);
        }
    }
}
