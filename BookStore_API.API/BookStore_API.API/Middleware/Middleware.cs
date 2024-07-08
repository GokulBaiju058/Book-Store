using BookStore_API.API.Extensions;
using BookStore_API.Business.Abstractions;
using BookStore_API.Infrastructure.Exceptions;
using Newtonsoft.Json;
using System.Net;

namespace BookStore_API.API.Middleware
{

    /// <summary>
    /// Middleware for handling exceptions and logging API requests.
    /// </summary>
    public class Middleware
    {
        private readonly RequestDelegate _next;
        private readonly IExceptionLoggerService _exceptionLogger;
        private readonly ILogger<Middleware> _logger;
        private readonly IAPILoggerService _apiLogger;
        private readonly IConfiguration _configuration;

        public Middleware(RequestDelegate next, IExceptionLoggerService exceptionLogger, ILogger<Middleware> logger, IAPILoggerService apiLogger, IConfiguration configuration)
        {
            _next = next;
            _exceptionLogger = exceptionLogger;
            _logger = logger;
            _apiLogger = apiLogger;
            _configuration = configuration;
        }

        /// <summary>
        /// Invokes the middleware.
        /// </summary>
        public async Task Invoke(HttpContext context)
        {
            var path = context.Request.Path.Value;
            await LogRequest(context);

            try
            {
                // Call the next middleware in the pipeline
                await _next(context);
            }
            catch (Exception error)
            {
                // Handle exceptions and generate appropriate JSON response
                await HandleExceptionAsync(context, error);
            }
        }

        /// <summary>
        /// Logs incoming API requests.
        /// </summary>
        private async Task LogRequest(HttpContext context)
        {
            var path = context.Request.Path.Value;
            var query = context.Request.QueryString.Value;
            var method = context.Request.Method;
            var userAgent = context.Request.Headers.ContainsKey("User-Agent")
                ? context.Request.Headers["User-Agent"].ToString()
                : null;
            var host = context.Request.Headers.ContainsKey("Host")
                ? context.Request.Headers["Host"].ToString()
                : null;

            string headers = string.Empty;

            if (context.Request.Headers != null)
            {
                if (context.Request.Headers.Count > 0)
                    headers = JsonConvert.SerializeObject(context.Request.Headers);
            }

            long? userId = context.User != null ? context.User.GetUserId() : null;

            await _apiLogger.Log(path, query, method, userAgent, host, userId, headers);

        }

        /// <summary>
        /// Handles exceptions and returns JSON response.
        /// </summary>
        private async Task HandleExceptionAsync(HttpContext context, Exception error)
        {
            var response = context.Response;
            response.ContentType = "application/json";
            string result = "";

            var errorObject = new ErrorObject
            {
                path = context.Request.Path.Value,
                message = error?.Message,
            };

            var innerException = error?.InnerException;
            while (innerException != null)
            {
                errorObject.message += $"\r\n{innerException.Message}";
                innerException = innerException.InnerException;
            }

            switch (error)
            {
                case KeyNotFoundException e:
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    errorObject.status = (int)HttpStatusCode.NotFound;
                    result = JsonConvert.SerializeObject(errorObject);
                    break;
                case ValidationException e:
                    response.StatusCode = (int)HttpStatusCode.UnprocessableEntity;
                    errorObject.status = (int)HttpStatusCode.UnprocessableEntity;
                    ValidationErrorObject validationErrorObject = new ValidationErrorObject(errorObject);
                    validationErrorObject.validation = e.validationObject;
                    result = JsonConvert.SerializeObject(validationErrorObject);
                    break;
                case UnauthorizedAccessException e:
                    response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    errorObject.status = (int)HttpStatusCode.Unauthorized;
                    result = JsonConvert.SerializeObject(errorObject);
                    break;
                default:
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    errorObject.status = (int)HttpStatusCode.InternalServerError;
                    result = JsonConvert.SerializeObject(errorObject);
                    await WriteExceptionLog(context, error);
                    break;
            }

            await response.WriteAsync(result);
        }

        /// <summary>
        /// Writes exception details to the exception logger.
        /// </summary>
        private async Task WriteExceptionLog(HttpContext context, Exception e)
        {
            var path = context.Request.Path.Value;
            var query = context.Request.QueryString.Value;
            var userAgent = context.Request.Headers.ContainsKey("User-Agent")
                ? context.Request.Headers["User-Agent"].ToString()
                : null;
            string? body = null;

            if (context.Request.Method == "POST" || context.Request.Method == "PUT" || context.Request.Method == "PATCH")
            {
                var requestBody = context.Request.Body;
                if (requestBody.CanSeek)
                {
                    requestBody.Seek(0L, SeekOrigin.Begin);

                    var streamReader = new StreamReader(requestBody);
                    body = await streamReader.ReadToEndAsync();
                }
            }

            await _exceptionLogger.Log(e, path, query, body, userAgent);
        }

        /// <summary>
        /// The structure of an error response object.
        /// </summary>
        public class ErrorObject
        {
            [JsonProperty(Order = 1)]
            public int status { get; set; }
            [JsonProperty(Order = 2)]
            public string? path { get; set; }
            [JsonProperty(Order = 3)]
            public string? message { get; set; }

            public ErrorObject() { }

            /// <summary>
            /// The structure of a validation error response object.
            /// </summary>
            protected ErrorObject(ErrorObject error)
            {
                path = error.path;
                status = error.status;
                message = error.message;
            }
        }

        public class ValidationErrorObject : ErrorObject
        {
            [JsonProperty(Order = 4)]
            public dynamic? validation { get; set; }

            public ValidationErrorObject(ErrorObject errorObject) : base(errorObject) { }


        }
    }
}
