using BookStore_API.Business.Abstractions;
using BookStore_API.Data.Entity;
using IS_Toyo_MicroLearning_API.Business;
using System.Linq.Expressions;

namespace BookStore_API.Business.Strategies.BookSearchStrategy
{
    public class InventorySearchStrategy : IBookSearchStrategy
    {
        public Expression<Func<Book, bool>> BuildSearchExpression(string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
                return book => true;

            if (searchTerm.Equals("available", StringComparison.OrdinalIgnoreCase))
                return book => book.CurrentQty > 0 && book.IsActive == true;

            if (searchTerm.Equals("unavailable", StringComparison.OrdinalIgnoreCase))
                return book => book.CurrentQty == 0 || book.IsActive == false;

            return PredicateBuilder.Or(
                ExpressionBuilder.GetExpression<Book>("BookName", searchTerm),
                ExpressionBuilder.GetExpression<Book>("Author", searchTerm)
            );
        }
    }
}
