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
        private readonly ILogger<Middleware> _logger;
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="Middleware"/> class.
        /// </summary>
        /// <param name="next">The next middleware in the pipeline.</param>
        /// <param name="logger">The logger for logging information.</param>
        /// <param name="configuration">The configuration settings.</param>
        public Middleware(RequestDelegate next, ILogger<Middleware> logger, IConfiguration configuration)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        /// <summary>
        /// Invokes the middleware to handle HTTP requests.
        /// </summary>
        /// <param name="context">The HTTP context.</param>
        public async Task Invoke(HttpContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var path = context.Request.Path.Value;

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
        /// Handles exceptions and returns JSON response.
        /// </summary>
        /// <param name="context">The HTTP context.</param>
        /// <param name="error">The exception that occurred.</param>
        private async Task HandleExceptionAsync(HttpContext context, Exception error)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

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
                    break;
            }

            await response.WriteAsync(result);
        }

        /// <summary>
        /// Represents the structure of an error response object.
        /// </summary>
        public class ErrorObject
        {
            /// <summary>
            /// Gets or sets the HTTP status code.
            /// </summary>
            [JsonProperty(Order = 1)]
            public int status { get; set; }

            /// <summary>
            /// Gets or sets the request path where the error occurred.
            /// </summary>
            [JsonProperty(Order = 2)]
            public string? path { get; set; }

            /// <summary>
            /// Gets or sets the error message.
            /// </summary>
            [JsonProperty(Order = 3)]
            public string? message { get; set; }

            /// <summary>
            /// Initializes a new instance of the <see cref="ErrorObject"/> class.
            /// </summary>
            public ErrorObject() { }

            /// <summary>
            /// Initializes a new instance of the <see cref="ErrorObject"/> class with specified values.
            /// </summary>
            /// <param name="error">The error object containing status, path, and message.</param>
            protected ErrorObject(ErrorObject error)
            {
                path = error.path;
                status = error.status;
                message = error.message;
            }
        }

        /// <summary>
        /// Represents the structure of a validation error response object.
        /// </summary>
        public class ValidationErrorObject : ErrorObject
        {
            /// <summary>
            /// Gets or sets the validation details.
            /// </summary>
            [JsonProperty(Order = 4)]
            public dynamic? validation { get; set; }

            /// <summary>
            /// Initializes a new instance of the <see cref="ValidationErrorObject"/> class.
            /// </summary>
            /// <param name="errorObject">The error object containing status, path, and message.</param>
            public ValidationErrorObject(ErrorObject errorObject) : base(errorObject) { }
        }
    }
}
