

using BookStore_API.Business.Abstractions;
using BookStore_API.Data;
using BookStore_API.Data.Entity;
using Microsoft.Extensions.DependencyInjection;

namespace BookStore_API.Business.Services.Logger
{
    /// <summary>
    /// Service for logging API requests.
    /// </summary>
    public class APILoggerService:IAPILoggerService
    {
        private readonly IServiceProvider _serviceProvider;

        public APILoggerService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Logs API request details asynchronously.
        /// </summary>
        /// <param name="requestPath">The path of the API request.</param>
        /// <param name="requestQueryString">The query string of the API request.</param>
        /// <param name="method">The HTTP method of the API request.</param>
        /// <param name="userAgent">The user agent of the client making the API request.</param>
        /// <param name="host">The host of the API request.</param>
        /// <param name="userId">The ID of the user making the API request (if authenticated).</param>
        /// <param name="headers">The headers of the API request.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task Log(string? requestPath, string? requestQueryString, string? method, string? userAgent, string? host, long? userId, string? headers)
        {
            using (var scope = _serviceProvider.CreateScope())
            using (var context = scope.ServiceProvider.GetService<BookStore_APIContext>())
            {

                if (context != null)
                {
                    context.APILogs.Add(new APILog
                    {
                        Path = requestPath,
                        QueryString = requestQueryString,
                        Method = method,
                        UserAgent = userAgent,
                        Host = host,
                        UserId = userId,
                        Headers = headers
                    });

                    await context.SaveChangesAsync();
                }
            }
        }
    }
}
