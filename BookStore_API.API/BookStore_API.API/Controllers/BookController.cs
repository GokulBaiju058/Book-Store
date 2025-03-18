using BookStore_API.API.Extensions;
using BookStore_API.Business.Abstractions;
using BookStore_API.Data;
using BookStore_API.Data.Entity;
using BookStore_API.Data.Enum;
using BookStore_API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookStore_API.API.Controllers
{
    /// <summary>
    /// Controller for managing book operations.
    /// </summary>
    [Route("api/book")]
    [ApiController]
    [Authorize] // Requires authorization for all actions in this controller
    public class BookController : ControllerBase
    {
        private readonly ILogger<BookController> _logger;
        private readonly IBookService _bookService;

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
        /// Retrieves a specific book by its identifier.
        /// </summary>
        /// <param name="bookId">The unique identifier of the book.</param>
        /// <returns>The book details.</returns>
        [HttpGet("{bookId}")]
        public async Task<ActionResult<ResponseMessage<BookViewDto>>> GetBook(int bookId)
        {
            _logger.LogInformation($"BookController - Single Book Details For BookId - " + bookId.ToString());
            return await _bookService.GetAsync(bookId);
        }

        /// <summary>
        /// Retrieves a collection of books with optional filtering and pagination.
        /// </summary>
        /// <param name="pageNumber">Page number for pagination.</param>
        /// <param name="pageSize">Number of items per page.</param>
        /// <param name="isActive">Filter by active status.</param>
        /// <param name="orderBy">Field to order by.</param>
        /// <param name="orderDirection">Order direction (true for ascending, false for descending).</param>
        /// <param name="search">Search criteria.</param>
        /// <param name="searchType">Determines the search strategy to use. Standard searches across multiple fields, while Inventory provides status-based filtering.</param>
        /// <returns>A paged list of books that match the specified criteria wrapped in a response message.</returns>
        [HttpGet]
        public ActionResult<ResponseMessage<PagedList<BookViewDto>>> GetBooks(
         [FromQuery] int? pageNumber,
         [FromQuery] int? pageSize,
         [FromQuery] bool? isActive = null,
         [FromQuery] string orderBy = "Id",
         [FromQuery] bool orderDirection = true,
         [FromQuery] string search = "",
         [FromQuery] SearchTypeEnum searchType = SearchTypeEnum.Standard)
        {
            _logger.LogWarning("Getting User Details");
            var userRole = HttpContext.User.GetRoles();
            if (userRole.Contains("User"))
            {
                isActive = true;
            }
            _logger.LogInformation($"BookController - GetAll Books with search type: {searchType}");
            return _bookService.GetAll(pageNumber, pageSize, isActive, orderBy, orderDirection, search, searchType);
        }

        /// <summary>
        /// Creates a book loan for the authenticated user.
        /// </summary>
        /// <param name="bookId">The identifier of the book to borrow.</param>
        /// <returns>The created loan record.</returns>
        [HttpPost("{bookId}/borrow")]
        public async Task<ActionResult<ResponseMessage<BorrowedBookViewDto>>> BorrowBook(int bookId)
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
        [Route("return/{borrowId}")]
        public async Task<ResponseMessage<BorrowedBook>> ReturnBook(int borrowId)
        {
            _logger.LogInformation($"BookController - Return Book.");
            return await _bookService.ReturnBook(borrowId);
        }
    }
}
