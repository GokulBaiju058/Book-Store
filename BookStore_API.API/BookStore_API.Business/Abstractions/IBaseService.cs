
using BookStore_API.Data;
using BookStore_API.Models;

namespace BookStore_API.Business.Abstractions
{
    /// <summary>
    /// Interface defining basic CRUD operations for a service.
    /// </summary>
    /// <typeparam name="T">Entity type.</typeparam>
    /// <typeparam name="TAdd">DTO type for adding.</typeparam>
    /// <typeparam name="TView">DTO type for viewing.</typeparam>
    public interface IBaseService<T, TAdd, TView>
    {
        /// <summary>
        /// Retrieves an entity by its ID.
        /// </summary>
        Task<ResponseMessage<TView>> GetAsync(int id);
        /// <summary>
        /// Retrieves a paged list of entities based on optional filters.
        /// </summary
        ResponseMessage<PagedList<T>> GetAll(int? pageNumber, int? pageSize, string orderBy, bool orderDirection, string search);
        /// <summary>
        /// Adds a new entity to the system.
        /// </summary>
        Task<ResponseMessage<T>> AddAsync(TAdd t);
        /// <summary>
        /// Updates an existing entity in the system.
        /// </summary>
        Task<ResponseMessage<T>> UpdateAsync(T t);
        /// <summary>
        /// Deletes an entity from the system by its ID.
        /// </summary>
        Task<ResponseMessage<bool>> DeleteAsync(int id);
    }
}
