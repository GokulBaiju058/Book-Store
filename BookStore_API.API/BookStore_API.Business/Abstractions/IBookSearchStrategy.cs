using BookStore_API.Data.Entity;
using System.Linq.Expressions;

namespace BookStore_API.Business.Abstractions
{
    public interface IBookSearchStrategy
    {
        Expression<Func<Book, bool>> BuildSearchExpression(string searchTerm);
    }
}
