using System.Globalization;

namespace BookStore_API.Infrastructure.Exceptions
{
    /// <summary>
    /// Represents an exception thrown when validation fails in the application.
    /// </summary>
    public class ValidationException : Exception
    {
        /// <summary>
        /// Additional validation details associated with the exception.
        /// </summary>
        public dynamic? validationObject { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationException"/> class.
        /// </summary>
        public ValidationException() : base() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationException"/> class with a specified error message.
        /// </summary>
        public ValidationException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationException"/> class with a formatted error message.
        /// </summary>
        public ValidationException(string message, params object[] args) : base(String.Format(CultureInfo.CurrentCulture, message, args))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationException"/> class with a specified error message and validation details.
        /// </summary>
        public ValidationException(string message, dynamic fields) : base(message)
        {
            validationObject = fields;
        }
    }
}
