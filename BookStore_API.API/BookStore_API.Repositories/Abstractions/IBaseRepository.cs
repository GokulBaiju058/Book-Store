using System.Linq.Expressions;
using BookStore_API.Data;

namespace BookStore_API.Repositories.Abstractions
{
    public interface IBaseRepository<TEntity>
         where TEntity : class
    {
        Task<TEntity> InsertOrUpdateAsync(int id, TEntity entity);

        Task<TEntity> InsertAsync(TEntity entity);

        Task UpdateAsync(TEntity entity);

        Task SoftDeleteAsync(TEntity entity);

        Task<IList<TEntity>> GetAllAsync();

        IQueryable<TEntity> GetAll();

        IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>> expression, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null, params Expression<Func<TEntity, object>>[] includes);

        Task<TEntity> GetByIdAsync(int id);

        IQueryable<TEntity> Get(Expression<Func<TEntity, bool>> expression);

        PagedList<TEntity> GetAll(int? pageNumber, int? pageSize, string orderBy, bool orderDirection, Expression<Func<TEntity, bool>> expression, Expression<Func<TEntity, bool>> searchExpression, params Expression<Func<TEntity, object>>[] includes);

        PagedList<TEntity> GetAll(int? pageNumber, int? pageSize, string orderBy, bool orderDirection, Expression<Func<TEntity, bool>> searchExpression, params Expression<Func<TEntity, object>>[] includes);

        PagedList<TEntity> GetAll(int? pageNumber, int? pageSize, string orderBy, bool orderDirection);
    }
}
