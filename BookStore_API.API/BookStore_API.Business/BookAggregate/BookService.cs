using BookStore_API.Business.Abstractions;
using BookStore_API.Business.Extensions;
using BookStore_API.Data.Entity;
using BookStore_API.Models;
using BookStore_API.Repositories.Abstractions;
using Microsoft.EntityFrameworkCore;
using Mapster;
using IS_Toyo_MicroLearning_API.Business;
using BookStore_API.Data;
using System.Linq.Expressions;

namespace BookStore_API.Business.BookAggregate
{
    /// <summary>
    /// Service class for handling operations related to books.
    /// </summary>
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;
        private readonly IBorrowedBookRepository _borrowedBookRepository;

        public BookService(IBookRepository bookRepository, IBorrowedBookRepository borrowedBookRepository)
        {
            _bookRepository = bookRepository ?? throw new ArgumentNullException(nameof(bookRepository));
            _borrowedBookRepository = borrowedBookRepository ?? throw new ArgumentNullException(nameof(borrowedBookRepository));
        }

        /// <summary>
        /// Retrieves a book by its ID.
        /// </summary>
        public async Task<ResponseMessage<BookViewDto>> GetAsync(int id)
        {
            Book book = await _bookRepository.GetByIdAsync(id);
            if (book == null)
            {
                return new ResponseMessage<BookViewDto>() { Data = null, Message = $"No Book Found With Id - {id}", Success = false };
            }

            return new ResponseMessage<BookViewDto>() { Data = book.Adapt<BookViewDto>() };
        }

        /// <summary>
        /// Retrieves detailed information about a book.
        /// </summary>
        public async Task<ResponseMessage<BookDetailViewDto>> GetBookDetail(int id)
        {
            return await _bookRepository.GetBookDetails(id);
        }

        /// <summary>
        /// Retrieves a list of books based on optional filtering and paging parameters.
        /// </summary>
        public ResponseMessage<PagedList<BookViewDto>> GetAll(int? pageNumber, int? pageSize,bool? isActive, string orderBy, bool orderDirection, string search)
        {
            Expression<Func<Book, bool>> expression = PredicateBuilder.Or(ExpressionBuilder.GetExpression<Book>("BookName", search), ExpressionBuilder.GetExpression<Book>("Author", search));
            var predicate = PredicateBuilder.True<Book>();

            // Add isActive filter if isActive is not null
            if (isActive.HasValue)
            {
                predicate = predicate.And(p => p.IsActive == isActive);
            }
            var books = _bookRepository.GetAll(pageNumber, pageSize, orderBy, orderDirection,predicate, expression);
            return new ResponseMessage<PagedList<BookViewDto>> { Data = books.Adapt<PagedList<BookViewDto>>() };
        }

        /// <summary>
        /// Adds a new book to the database.
        /// </summary>
        public async Task<ResponseMessage<BookDto>> AddAsync(BookDto book)
        {
            if (book == null)
            {
                throw new ArgumentNullException(nameof(book));
            }

            new ValidatorExtensions.GenericValidation<BookDto, BookValidator>().Validate(book);
            Book saveBook = book.Adapt<Book>();
            var newBook = await _bookRepository.InsertAsync(saveBook);
            return new ResponseMessage<BookDto>() { Data = newBook.Adapt<BookDto>() };
        }

        /// <summary>
        /// Updates an existing book in the database.
        /// </summary>
        public async Task<ResponseMessage<BookDto>> UpdateAsync(BookDto book)
        {
            if (book == null)
            {
                return new ResponseMessage<BookDto>() { Success = false, Message = "Invalid Object" };
            }

            new ValidatorExtensions.GenericValidation<BookDto, BookValidator>().Validate(book);
            var existingBook = await _bookRepository.GetByIdAsync(book.Id);
            if (existingBook == null)
            {
                return new ResponseMessage<BookDto>() { Success = false, Message = "No Existing Book Found" };
            }

            var updatedBook = await _bookRepository.InsertOrUpdateAsync(book.Id, book.Adapt<Book>());
            return new ResponseMessage<BookDto>() { Data = updatedBook.Adapt<BookDto>() };
        }

        /// <summary>
        /// Deletes a book from the database.
        /// </summary>
        public async Task<ResponseMessage<bool>> DeleteAsync(int id)
        {
            Book book = await _bookRepository.GetByIdAsync(id);

            if (book == null)
            {
                return new ResponseMessage<bool>() { Data = false, Message = $"No Book Found With Id - {id}", Success = false };
            }

            //TODO Implement logic for what to do if the book has more than 1 QTY
            await _bookRepository.SoftDeleteAsync(book);
            return new ResponseMessage<bool>() { Data = true };
        }

        /// <summary>
        /// Handles the borrowing of a book by a user.
        /// </summary>
        public async Task<ResponseMessage<BorrowedBookViewDto>> BorrowBook(BorrowBookDto borrowBookDto)
        {
            if (borrowBookDto == null)
            {
                throw new ArgumentNullException(nameof(borrowBookDto));
            }

            new ValidatorExtensions.GenericValidation<BorrowBookDto, BorrowBookValidator>().Validate(borrowBookDto);

            // Check if the book exists and Available to borrow
            var book = await _bookRepository.Get(x => x.Id == borrowBookDto.BookId && x.CurrentQty > 0).SingleOrDefaultAsync();
            if (book == null)
            {
                return new ResponseMessage<BorrowedBookViewDto>() { Message = "Book Not Found", Success = false };
            }

            // Check if the book is already borrowed by the user
            var borrowed = await _borrowedBookRepository.Get(x => x.UserId == borrowBookDto.UserId && x.BookId == borrowBookDto.BookId && x.ReturnedDate == null).SingleOrDefaultAsync();
            if (borrowed != null)
            {
                return new ResponseMessage<BorrowedBookViewDto>() { Message = "Book Already Borrowed", Success = false };
            }

            // Decrease the current quantity of the book
            book.CurrentQty -= 1;
            await _bookRepository.UpdateAsync(book);

            // Save the borrowed book details
            BorrowedBook saveBorrow = borrowBookDto.Adapt<BorrowedBook>();
            saveBorrow.BorrowedDate = DateTime.Now;
            saveBorrow.Remarks = borrowBookDto.Remarks;
            var newBorrow = await _borrowedBookRepository.InsertAsync(saveBorrow);

            return new ResponseMessage<BorrowedBookViewDto>() { Data = newBorrow.Adapt<BorrowedBookViewDto>() };
        }

        /// <summary>
        /// Handles the return of a borrowed book.
        /// </summary>
        public async Task<ResponseMessage<BorrowedBook>> ReturnBook(int borrowId)
        {
            var borrowedBook = await _borrowedBookRepository.Get(x => x.Id == borrowId).SingleOrDefaultAsync();
            if (borrowedBook == null)
            {
                return new ResponseMessage<BorrowedBook>() { Message = "Record Not Found", Success = false };
            }

            var existingBook = await _bookRepository.Get(x => x.Id == borrowedBook.BookId).SingleOrDefaultAsync();
            if (existingBook == null)
            {
                return new ResponseMessage<BorrowedBook>() { Message = "Associated Book Not Found", Success = false };
            }

            existingBook.CurrentQty += 1;
            var updatedBook = await _bookRepository.InsertOrUpdateAsync(existingBook.Id, existingBook);

            borrowedBook.ReturnedDate = DateTime.Now;
            var updatedBorrow = await _borrowedBookRepository.InsertOrUpdateAsync(borrowedBook.Id, borrowedBook);

            return new ResponseMessage<BorrowedBook>() { Data = updatedBook.Adapt<BorrowedBook>() };
        }

        /// <summary>
        /// Retrieves a paged list of borrowed books based on optional filtering and paging parameters.
        /// </summary>
        public ResponseMessage<PagedList<BorrowedBook>> GetAllBorrowedBooks(int? pageNumber, int? pageSize, string orderBy, bool orderDirection, string search)
        {
            var expression = ExpressionBuilder.GetExpression<BorrowedBook>("Remarks", search);
            var borrowedBooks = _borrowedBookRepository.GetAll(pageNumber, pageSize, orderBy, orderDirection, expression, includes: g => g.Book);
            return new ResponseMessage<PagedList<BorrowedBook>>() { Data = borrowedBooks.Adapt<PagedList<BorrowedBook>>() };
        }

        /// <summary>
        /// Retrieves a list of borrowed books by a specific user.
        /// </summary>
        public ResponseMessage<List<BorrowedBookViewDto>> GetAllBorrowedBookByUser(int userId)
        {
            var borrowedBooks = _borrowedBookRepository.Get(x => x.UserId == userId, includes: g => g.Book).ToList();
            if (borrowedBooks.Count == 0)
            {
                return new ResponseMessage<List<BorrowedBookViewDto>>() { Data = null, Success = false, Message = "No Data found" };
            }
            return new ResponseMessage<List<BorrowedBookViewDto>>() { Data = borrowedBooks.Adapt<List<BorrowedBookViewDto>>() };
        }
    }
}
