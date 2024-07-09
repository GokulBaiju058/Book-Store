using BookStore_API.API.Extensions;
using BookStore_API.Business.Abstractions;
using BookStore_API.Data.Entity;
using BookStore_API.Data;
using BookStore_API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace BookStore_API.API.Controllers
{
    /// <summary>
    /// Controller for managing book operations.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Requires authorization for all actions in this controller
    public class BookController : ControllerBase
    {
        private readonly ILogger<BookController> _logger; // Logger for logging information
        private readonly IBookService _bookService; // Service for managing books

        /// <summary>
        /// Initializes a new instance of the <see cref="BookController"/> class.
        /// </summary>
        /// <param name="logger">Logger instance for logging information related to book operations.</param>
        /// <param name="bookService">Service interface providing methods for book management.</param>
        public BookController(ILogger<BookController> logger, IBookService bookService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _bookService = bookService ?? throw new ArgumentNullException(nameof(bookService));
        }

        /// <summary>
        /// Retrieves details of a book by its Id.
        /// </summary>
        /// <param name="bookId">The Id of the book.</param>
        [HttpGet]
        [Route("GetbookById")]
        public async Task<ResponseMessage<BookViewDto>> GetbookById(int bookId)
        {
            _logger.LogInformation($"BookController - Single Book Details For BookId - " + bookId.ToString());
            return await _bookService.GetAsync(bookId);
        }

        /// <summary>
        /// Retrieves a paged list of all books.
        /// </summary>
        /// <param name="pageNumber">Page number.</param>
        /// <param name="pageSize">Number of items per page.</param>
        /// <param name="orderBy">Field to order by.</param>
        /// <param name="orderDirection">Order direction (ascending or descending).</param>
        /// <param name="search">Search criteria.</param>
        [HttpGet]
        [Route("GetAllBooks")]
        public ResponseMessage<PagedList<BookViewDto>> GetAll(int? pageNumber, int? pageSize, string orderBy = "Id", bool orderDirection = true, string search = "")
        {
            _logger.LogInformation("BookController - GetAll Books");
            return _bookService.GetAll(pageNumber, pageSize, orderBy, orderDirection, search);
        }

        /// <summary>
        /// Borrows a book for the authenticated user.
        /// </summary>
        /// <param name="bookId">The Id of the book to borrow.</param>
        [HttpPost]
        [Route("BorrowBook")]
        public async Task<ResponseMessage<BorrowedBookViewDto>> BorrowBook(int bookId)
        {
            _logger.LogWarning("Getting User Details");
            int userId = HttpContext.User.GetUserId();
            _logger.LogInformation($"BookController - Borrow Book.");
            BorrowBookDto borrowBookDto = new BorrowBookDto() { BookId = bookId, UserId = userId };
            return await _bookService.BorrowBook(borrowBookDto);
        }

        /// <summary>
        /// Returns a borrowed book by its borrow Id.
        /// </summary>
        /// <param name="borrowId">The Id of the borrow record.</param>
        [HttpPost]
        [Route("ReturnBook")]
        public async Task<ResponseMessage<BorrowedBook>> ReturnBook(int borrowId)
        {
            _logger.LogInformation($"BookController - Return Book.");
            return await _bookService.ReturnBook(borrowId);
        }
    }
}
