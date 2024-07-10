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
    [Route("api/[controller]")]
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
        /// Adds a new book.
        /// </summary>
        /// <param name="book">The book details to add.</param>
        [HttpPost]
        [Route("AddBook")]
        public async Task<ResponseMessage<BookDto>> AddBook(BookDto book)
        {
            _logger.LogInformation("BookController - Create New Book."); // Log Book Adding attempt
            return await _bookService.AddAsync(book);
        }

        /// <summary>
        /// Updates an existing book.
        /// </summary>
        /// <param name="book">The updated book details.</param>
        [HttpPut]
        [Route("UpdateBook")]
        public async Task<ResponseMessage<BookDto>> UpdateBook(BookDto book)
        {
            _logger.LogInformation("BookController - Update Book."); // Log Update Book attempt
            return await _bookService.UpdateAsync(book);
        }

        /// <summary>
        /// Deletes a book by its Id.
        /// </summary>
        /// <param name="bookId">The Id of the book to delete.</param>
        [HttpDelete]
        [Route("DeleteBook")]
        public async Task<ResponseMessage<bool>> DeleteBook(int bookId)
        {
            _logger.LogInformation("BookController - Delete Book with BookId - " + bookId.ToString()); // Log Delete Book attempt
            return await _bookService.DeleteAsync(bookId);
        }

        /// <summary>
        /// Retrieves all books borrowed by a specific user.
        /// </summary>
        /// <param name="userId">The Id of the user.</param>
        [HttpPost]
        [Route("GetAllBorrowedBookByUser")]
        public ResponseMessage<List<BorrowedBookViewDto>> GetAllBorrowedBookByUser(int userId)
        {
            _logger.LogInformation($"BookController - Get books Borrowed By a User.");
            return _bookService.GetAllBorrowedBookByUser(userId);
        }

        /// <summary>
        /// Retrieves a paged list of all borrowed books.
        /// </summary>
        /// <param name="pageNumber">Page number.</param>
        /// <param name="pageSize">Number of items per page.</param>
        /// <param name="orderBy">Field to order by.</param>
        /// <param name="orderDirection">Order direction (ascending or descending).</param>
        /// <param name="search">Search criteria.</param>
        [HttpGet]
        [Route("GetAllBorrowedBooks")]
        public ResponseMessage<PagedList<BorrowedBook>> GetAllBorrowedBooks(int? pageNumber, int? pageSize, string orderBy = "Id", bool orderDirection = true, string search = "")
        {
            return _bookService.GetAllBorrowedBooks(pageNumber, pageSize, orderBy, orderDirection, search);
        }

        /// <summary>
        /// Retrieves detailed information about a specific book.
        /// </summary>
        /// <param name="bookId">The Id of the book.</param>
        [HttpGet]
        [Route("GetBookDetail")]
        public async Task<ResponseMessage<BookDetailViewDto>> GetBookDetail(int bookId)
        {
            _logger.LogInformation($"BookController - Single Book Details For BookId - " + bookId.ToString());
            return await _bookService.GetBookDetail(bookId);
        }
    }
}
