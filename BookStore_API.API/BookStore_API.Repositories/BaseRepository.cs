using BookStore_API.Data;
using BookStore_API.Repositories.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace BookStore_API.Repositories
{

    public abstract class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class
    {
        protected readonly IUnitOfWork _unitOfWork; // Unit of work instance for database operations
        private readonly IHttpContextAccessor _httpContextAccessor; // Accessor for HTTP context information
        private readonly ILogger<BaseRepository<TEntity>> _logger; // Logger for logging operations

        public BaseRepository(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, ILogger<BaseRepository<TEntity>> logger)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        // Method: InsertOrUpdateAsync
        // Summary: Inserts or updates an entity asynchronously based on ID.
        // Parameters:
        // - id: The ID of the entity to be updated.
        // - entity: The entity object to be inserted or updated.
        // Returns: The inserted or updated entity object.
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

        // Method: InsertAsync
        // Summary: Inserts a new entity asynchronously.
        // Parameters:
        // - entity: The entity object to be inserted.
        // Returns: The inserted entity object.
        public async Task<TEntity> InsertAsync(TEntity entity)
        {
            _logger.LogInformation("BaseRepository - InsertAsync - Entering - User Id - " + GetUserId().ToString());

            await _unitOfWork._context.Set<TEntity>().AddAsync(entity);
            await _unitOfWork.CommitAsync();
            return entity;
        }

        // Method: UpdateAsync
        // Summary: Updates an existing entity asynchronously.
        // Parameters:
        // - entity: The entity object to be updated.
        // Returns: Task representing the asynchronous operation.
        public async Task UpdateAsync(TEntity entity)
        {
            _logger.LogInformation("BaseRepository - UpdateAsync - Entering -  User Id - " + GetUserId().ToString());

            _unitOfWork._context.Set<TEntity>().Update(entity);
            await _unitOfWork.CommitAsync();
        }

        // Method: SoftDeleteAsync
        // Summary: Soft deletes an entity asynchronously by setting IsActive property to false.
        // Parameters:
        // - entity: The entity object to be soft deleted.
        // Returns: Task representing the asynchronous operation.
        public async Task SoftDeleteAsync(TEntity entity)
        {
            _logger.LogInformation("BaseRepository - SoftDeleteAsync - Entering -  User Id - " + GetUserId().ToString());

            var isActiveProperty = entity.GetType().GetProperty("IsActive");
            isActiveProperty?.SetValue(entity, false, null);

            _unitOfWork._context.Set<TEntity>().Update(entity);
            await _unitOfWork.CommitAsync();
        }

        // Method: GetAll
        // Summary: Retrieves all entities of type TEntity from the database.
        // Returns: IQueryable<TEntity> representing all entities.
        public IQueryable<TEntity> GetAll() => _unitOfWork._context.Set<TEntity>().AsQueryable();

        // Method: GetAllAsync
        // Summary: Retrieves all entities of type TEntity asynchronously from the database.
        // Returns: Task<IList<TEntity>> representing a list of all entities.
        public async Task<IList<TEntity>> GetAllAsync()
        {
            return await _unitOfWork._context.Set<TEntity>().ToListAsync();
        }

        // Method: Get
        // Summary: Retrieves entities of type TEntity based on filter conditions, with optional includes and ordering.
        // Parameters:
        // - expression: The filter expression to apply on entities.
        // - orderBy: Optional function for ordering entities.
        // - includes: Optional array of expressions for including related entities.
        // Returns: IEnumerable<TEntity> representing the retrieved entities.
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

        // Method: GetAll
        // Summary: Retrieves entities of type TEntity with pagination, ordering, and search conditions.
        // Parameters:
        // - pageNumber: The page number of entities to retrieve.
        // - pageSize: The number of entities per page.
        // - orderBy: The field to order entities by.
        // - orderDirection: The order direction (ascending or descending).
        // - searchExpression: Optional filter expression for additional search conditions.
        // - includes: Optional array of expressions for including related entities.
        // Returns: PagedList<TEntity> representing paged list of entities.
        public PagedList<TEntity> GetAll(int? pageNumber, int? pageSize, string orderBy, bool orderDirection, Expression<Func<TEntity, bool>> searchExpression, params Expression<Func<TEntity, object>>[] includes)
        {
            return GetAll(pageNumber, pageSize, orderBy, orderDirection, searchExpression, null, includes);
        }

        // Method: GetAll
        // Summary: Retrieves entities of type TEntity with pagination, ordering, and search conditions.
        // Parameters:
        // - pageNumber: The page number of entities to retrieve.
        // - pageSize: The number of entities per page.
        // - orderBy: The field to order entities by.
        // - orderDirection: The order direction (ascending or descending).
        // Returns: PagedList<TEntity> representing paged list of entities.
        public PagedList<TEntity> GetAll(int? pageNumber, int? pageSize, string orderBy, bool orderDirection)
        {
            return GetAll(pageNumber, pageSize, orderBy, orderDirection, null, null, null);
        }

        // Method: GetByIdAsync
        // Summary: Retrieves an entity of type TEntity by its ID asynchronously.
        // Parameters:
        // - id: The ID of the entity to retrieve.
        // Returns: Task<TEntity> representing the retrieved entity.
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

        // Method: Get
        // Summary: Retrieves entities of type TEntity based on filter conditions.
        // Parameters:
        // - expression: The filter expression to apply on entities.
        // Returns: IQueryable<TEntity> representing the retrieved entities.
        public virtual IQueryable<TEntity> Get(Expression<Func<TEntity, bool>> expression) => GetAll().Where(expression);

        // Method: GetAll
        // Summary: Retrieves entities of type TEntity with pagination, ordering, and search conditions.
        // Parameters:
        // - pageNumber: The page number of entities to retrieve.
        // - pageSize: The number of entities per page.
        // - orderBy: The field to order entities by.
        // - orderDirection: The order direction (ascending or descending).
        // - expression: Optional filter expression for additional conditions.
        // - searchExpression: Optional filter expression for additional search conditions.
        // - includes: Optional array of expressions for including related entities.
        // Returns: PagedList<TEntity> representing paged list of entities.
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

            return PagedList<TEntity>.ToPagedList(query.AsQueryable(), pageNumber.Value, pageSize.Value, orderBy, orderDirection);

        }

        private long GetUserId()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.Claims?.FirstOrDefault(c => c.Type == "userId");
            if (userIdClaim != null && long.TryParse(userIdClaim.Value, out long userId))
            {
                return userId;
            }
            return 0; // Or any other appropriate default value
        }
    }
}
