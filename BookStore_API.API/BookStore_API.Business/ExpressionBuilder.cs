using System.Linq.Expressions;
using System.Reflection;

namespace IS_Toyo_MicroLearning_API.Business
{
    public static class ExpressionBuilder
    {

        public static Expression<Func<T, bool>> GetExpression<T>(string propertyName, string search)
        {
            if (string.IsNullOrEmpty(search))
                return null;

            if (string.IsNullOrEmpty(propertyName))
                return null;

            var property = typeof(T).GetProperty(propertyName);
            var parent = Expression.Parameter(typeof(T));
            MethodInfo method = typeof(string).GetMethod("Contains", new Type[] { typeof(String) });
            var expressionBody = Expression.Call(Expression.Property(parent, property), method, Expression.Constant(search));
            return Expression.Lambda<Func<T, bool>>(expressionBody, parent);
        }
    }

    public static class PredicateBuilder
    {
        public static Expression<Func<T, bool>> True<T>() { return f => true; }
        public static Expression<Func<T, bool>> False<T>() { return f => false; }

        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
        {
            if (expr1 == null || expr2 == null)
                return null;

            var invokedExpr = Expression.Invoke(expr2, expr1.Parameters.Cast<Expression>());
            return Expression.Lambda<Func<T, bool>>
                  (Expression.OrElse(expr1.Body, invokedExpr), expr1.Parameters);
        }

        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
        {

            if (expr1 == null || expr2 == null)
                return null;

            var invokedExpr = Expression.Invoke(expr2, expr1.Parameters.Cast<Expression>());
            return Expression.Lambda<Func<T, bool>>
                  (Expression.AndAlso(expr1.Body, invokedExpr), expr1.Parameters);
        }
    }


}
