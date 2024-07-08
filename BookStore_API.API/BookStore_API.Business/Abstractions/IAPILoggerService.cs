
namespace BookStore_API.Business.Abstractions
{
    /// <summary>
    /// Interface for logging API interactions.
    /// </summary>
    public interface IAPILoggerService
    {
        /// <summary>
        /// Logs details of an API request asynchronously.
        /// </summary>
        /// <param name="requestPath">The path of the API request.</param>
        /// <param name="requestQueryString">The query string of the API request.</param>
        /// <param name="method">The HTTP method of the API request.</param>
        /// <param name="userAgent">The user agent of the client making the request.</param>
        /// <param name="host">The host of the API request.</param>
        /// <param name="userId">The ID of the user making the request.</param>
        /// <param name="headers">The headers of the API request.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task Log(string? requestPath, string? requestQueryString, string? method, string? userAgent, string? host, long? userId, string? headers);
    }
}
