using BookStore_API.Business.Abstractions;
using BookStore_API.Data.Entity;
using IS_Toyo_MicroLearning_API.Business;
using System.Linq.Expressions;

namespace BookStore_API.Business.Strategies.BookSearchStrategy
{
    public class StandardSearchStrategy : IBookSearchStrategy
    {
        public Expression<Func<Book, bool>> BuildSearchExpression(string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
                return PredicateBuilder.True<Book>();

            return PredicateBuilder.Or(
                PredicateBuilder.Or(
                    ExpressionBuilder.GetExpression<Book>("BookName", searchTerm),
                    ExpressionBuilder.GetExpression<Book>("Author", searchTerm)
                ),
                ExpressionBuilder.GetExpression<Book>("Genre", searchTerm)
            );
        }
    }
}
