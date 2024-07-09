
using BookStore_API.Data;
using BookStore_API.Data.Entity;
using BookStore_API.Models;

namespace BookStore_API.Business.Abstractions
{
    public interface IBookService
    {
        Task<ResponseMessage<BookViewDto>> GetAsync(int id);
        Task<ResponseMessage<BookDetailViewDto>> GetBookDetail(int id);
        ResponseMessage<PagedList<BookViewDto>> GetAll(int? pageNumber, int? pageSize, string orderBy, bool orderDirection, string search);
        Task<ResponseMessage<BookDto>> AddAsync(BookDto addBook);
        Task<ResponseMessage<BookDto>> UpdateAsync(BookDto updateBook);
        Task<ResponseMessage<bool>> DeleteAsync(int id);
        Task<ResponseMessage<BorrowedBookViewDto>> BorrowBook(BorrowBookDto borrowBookDto);
        Task<ResponseMessage<BorrowedBook>> ReturnBook(int borrowId);
        ResponseMessage<PagedList<BorrowedBook>> GetAllBorrowedBooks(int? pageNumber, int? pageSize, string orderBy, bool orderDirection, string search);
        ResponseMessage<List<BorrowedBookViewDto>> GetAllBorrowedBookByUser(int userId);
    }
}
