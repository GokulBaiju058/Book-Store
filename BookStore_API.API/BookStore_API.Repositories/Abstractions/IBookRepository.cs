using BookStore_API.Data.Entity;
using BookStore_API.Models;

namespace BookStore_API.Repositories.Abstractions
{
    public interface IBookRepository:IBaseRepository<Book>
    {
        Task <ResponseMessage<BookDetailViewDto>> GetBookDetails(int bookId);
    }
}
