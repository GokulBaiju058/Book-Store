using BookStore_API.Data;
using BookStore_API.Repositories.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace BookStore_API.Repositories
{
    /// <summary>
    /// Base repository providing common CRUD operations for entities.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity managed by the repository.</typeparam>
    public abstract class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class
    {
        protected readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<BaseRepository<TEntity>> _logger;

        public BaseRepository(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, ILogger<BaseRepository<TEntity>> logger)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        /// <summary>
        /// Inserts or updates an entity in the database.
        /// </summary>
        public async Task<TEntity> InsertOrUpdateAsync(int id, TEntity entity)
        {

            _logger.LogInformation("BaseRepository - InsertOrUpdateAsync - Entering -  User Id - " + GetUserId().ToString());

            TEntity? getEntity = await _unitOfWork._context.Set<TEntity>().FindAsync(id);
            if (getEntity == null)
            {
                await _unitOfWork._context.Set<TEntity>().AddAsync(entity);
            }
            else
            {
                // detach
                _unitOfWork._context.Entry(getEntity).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
                _unitOfWork._context.Entry(entity).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                _unitOfWork._context.Set<TEntity>().Update(entity);
            }

            await _unitOfWork.CommitAsync();
            return entity;
        }

        /// <summary>
        /// Inserts an entity into the database.
        /// </summary>
        public async Task<TEntity> InsertAsync(TEntity entity)
        {
            _logger.LogInformation("BaseRepository - InsertAsync - Entering - User Id - " + GetUserId().ToString());

            await _unitOfWork._context.Set<TEntity>().AddAsync(entity);
            await _unitOfWork.CommitAsync();
            return entity;
        }

        /// <summary>
        /// Updates an entity in the database.
        /// </summary>
        public async Task UpdateAsync(TEntity entity)
        {
            _logger.LogInformation("BaseRepository - UpdateAsync - Entering -  User Id - " + GetUserId().ToString());

            _unitOfWork._context.Set<TEntity>().Update(entity);
            await _unitOfWork.CommitAsync();
        }

        /// <summary>
        /// Soft deletes an entity in the database.
        /// </summary>
        public async Task SoftDeleteAsync(TEntity entity)
        {
            _logger.LogInformation("BaseRepository - SoftDeleteAsync - Entering -  User Id - " + GetUserId().ToString());

            _unitOfWork._context.Set<TEntity>().Update(entity);
            await _unitOfWork.CommitAsync();
        }

        /// <summary>
        /// Retrieves all entities from the database.
        /// </summary>
        public IQueryable<TEntity> GetAll() => _unitOfWork._context.Set<TEntity>().AsQueryable();

        /// <summary>
        /// Retrieves all entities asynchronously from the database.
        /// </summary>
        public async Task<IList<TEntity>> GetAllAsync()
        {
            return await _unitOfWork._context.Set<TEntity>().ToListAsync();
        }

        public IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>> expression, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null, params Expression<Func<TEntity, object>>[] includes)
        {
            var query = GetAll().Where(expression);

            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }

            if (orderBy != null)
            {
                query = orderBy(query).AsQueryable();
            }

            return query;

        }

        public PagedList<TEntity> GetAll(int? pageNumber, int? pageSize, string orderBy, bool orderDirection, Expression<Func<TEntity, bool>> searchExpression, params Expression<Func<TEntity, object>>[] includes)
        {
            return GetAll(pageNumber, pageSize, orderBy, orderDirection, searchExpression, null, includes);
        }

        public PagedList<TEntity> GetAll(int? pageNumber, int? pageSize, string orderBy, bool orderDirection)
        {
            return GetAll(pageNumber, pageSize, orderBy, orderDirection, null, null, null);
        }

        public async Task<TEntity> GetByIdAsync(int id)
        {
            TEntity? entity = await _unitOfWork._context.Set<TEntity>().FindAsync(id);
            if (entity == null)
            {
                Type type = typeof(TEntity);
                throw new KeyNotFoundException("No " + type.Name + " object found with id: " + id.ToString());
            }

            return entity;
        }

        public virtual IQueryable<TEntity> Get(Expression<Func<TEntity, bool>> expression) => GetAll().Where(expression);

        public virtual IQueryable<TEntity> Include<TProperty>(IQueryable<TEntity> query, Expression<Func<TEntity, TProperty>> path) => query.Include(path);

        public async Task DeleteAsync(int id)
        {
            TEntity? entity = await _unitOfWork._context.Set<TEntity>().FindAsync(id);
            if (entity == null)
            {
                Type type = typeof(TEntity);
                throw new KeyNotFoundException("No " + type.Name + " object found with id: " + id.ToString());
            }

            _unitOfWork._context.Set<TEntity>().Remove(entity);
            await _unitOfWork.CommitAsync();
        }

        public PagedList<TEntity> GetAll(int? pageNumber, int? pageSize, string orderBy, bool orderDirection, Expression<Func<TEntity, bool>>? expression, Expression<Func<TEntity, bool>>? searchExpression, params Expression<Func<TEntity, object>>[]? includes)
        {

            IQueryable<TEntity> query = Enumerable.Empty<TEntity>().AsQueryable();

            if (expression != null)
            {
                query = GetAll().Where(expression);
            }
            else
            {
                query = GetAll();
            }

            if (searchExpression != null)
            {
                query = query.Where(searchExpression);
            }

            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }

            if (pageNumber == null)
                pageNumber = 1;

            if (pageSize == null)
                pageSize = 10;

            //var currentPageValues = allValues.Skip((pageNumber.Value - 1) * pageSize.Value)
            //.Take(pageSize.Value).AsQueryable();

            return PagedList<TEntity>.ToPagedList(query.AsQueryable(), pageNumber.Value, pageSize.Value, orderBy, orderDirection);

        }

        public async Task<bool> ExistAsync(Expression<Func<TEntity, bool>> expression) => await _unitOfWork._context.Set<TEntity>().AsNoTracking().AnyAsync(expression);

        public async Task<TEntity?> GetOneNoTracking(Expression<Func<TEntity, bool>> expression) => await _unitOfWork._context.Set<TEntity>().AsNoTracking().FirstOrDefaultAsync(expression);

        public async Task<TEntity?> GetOneTracking(Expression<Func<TEntity, bool>> expression) => await _unitOfWork._context.Set<TEntity>().FirstOrDefaultAsync(expression);
        public async Task<IReadOnlyList<TEntity>> GetPagedReponseAsync(int pageNumber, int pageSize)
        {
            return await _unitOfWork._context
                .Set<TEntity>()
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();
        }

        private long GetUserId()
        {
            long.TryParse(_httpContextAccessor.HttpContext.User.Claims?.FirstOrDefault(c => c.Type == "userId")?.Value, out long userId);
            return userId;
        }
    }
}
