using BookStore_API.Business.Abstractions;
using BookStore_API.Data;
using BookStore_API.Models;
using BookStore_API.Repositories.Abstractions;
using Mapster;

namespace BookStore_API.Business
{
    /// <summary>
    /// Base service implementing common operations for entities.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity.</typeparam>
    /// <typeparam name="TAddEntity">The type used for adding new entities.</typeparam>
    /// <typeparam name="TEntityView">The type used for viewing entities.</typeparam>
    public abstract class BaseService<TEntity, TAddEntity, TEntityView> : IBaseService<TEntity, TAddEntity, TEntityView>
      where TEntity : class
      where TAddEntity : class
    {
        private readonly IBaseRepository<TEntity> _repository;

        public BaseService(IBaseRepository<TEntity> repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Retrieves an entity by its identifier asynchronously.
        /// </summary>
        /// <param name="id">The identifier of the entity to retrieve.</param>
        /// <returns>A response message containing the retrieved entity view.</returns>
        public async Task<ResponseMessage<TEntityView>> GetAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            return new ResponseMessage<TEntityView>() { Data = entity.Adapt<TEntityView>() };
        }

        /// <summary>
        /// Deletes an entity by its identifier asynchronously.
        /// </summary>
        /// <param name="id">The identifier of the entity to delete.</param>
        /// <returns>A response message indicating the success of the deletion.</returns>
        public async Task<ResponseMessage<bool>> DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
            return new ResponseMessage<bool>() { Data = true };
        }

        /// <summary>
        /// Retrieves all entities optionally paginated, ordered, and filtered.
        /// </summary>
        /// <param name="pageNumber">The page number of results to retrieve.</param>
        /// <param name="pageSize">The size of each page of results.</param>
        /// <param name="orderBy">The property to order results by.</param>
        /// <param name="orderDirection">The direction of ordering (ascending or descending).</param>
        /// <param name="search">A search string to filter entities.</param>
        /// <returns>A response message containing a paged list of entities.</returns>
        public ResponseMessage<PagedList<TEntity>> GetAll(int? pageNumber, int? pageSize, string orderBy, bool orderDirection, string search)
        {
            var entities = _repository.GetAll(pageNumber, pageSize, orderBy, orderDirection, null);
            return new ResponseMessage<PagedList<TEntity>>() { Data = entities.Adapt<PagedList<TEntity>>() };
        }

        /// <summary>
        /// Adds a new entity asynchronously.
        /// </summary>
        /// <param name="addEntity">The entity to add.</param>
        /// <returns>A response message containing the added entity.</returns>
        public async Task<ResponseMessage<TEntity>> AddAsync(TAddEntity addEntity)
        {
            TEntity dataEntity = addEntity.Adapt<TEntity>();
            var newEntity = await _repository.InsertAsync(dataEntity);
            return new ResponseMessage<TEntity>() { Data = newEntity.Adapt<TEntity>() };
        }

        /// <summary>
        /// Updates an existing entity asynchronously.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        /// <returns>A response message containing the updated entity.</returns>
        public async Task<ResponseMessage<TEntity>> UpdateAsync(TEntity entity)
        {
            TEntity dataEntity = entity.Adapt<TEntity>();
            await _repository.UpdateAsync(dataEntity);
            return new ResponseMessage<TEntity>() { Data = dataEntity.Adapt<TEntity>() };
        }
    }
}
