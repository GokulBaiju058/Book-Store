using BookStore_API.Data.Entity;
using BookStore_API.Data;
using BookStore_API.Models;

namespace BookStore_API.Business.Abstractions
{
    /// <summary>
    /// Interface for logging exceptions and retrieving logged exceptions.
    /// </summary>
    public interface IExceptionLoggerService
    {
        Task Log(Exception exception, string? requestPath, string? requestQueryString, string? requestBody, string? userAgent);
        Task<ResponseMessage<PagedList<ExceptionLog>>> GetAllAsync(int? pageNumber, int? pageSize, string orderBy, bool orderDirection, string search);
    }
}
