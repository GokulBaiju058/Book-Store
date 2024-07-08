using BookStore_API.Infrastructure.Exceptions;
using FluentValidation.Results;
using System.Reflection;

namespace BookStore_API.Business.Extensions
{
    /// <summary>
    /// Contains extension classes for generic validation using FluentValidation.
    /// </summary>
    public static class ValidatorExtensions
    {
        /// <summary>
        /// Provides generic validation capabilities using FluentValidation for any request type.
        /// </summary>
        /// <typeparam name="TRequest">The type of request to validate.</typeparam>
        /// <typeparam name="TValidator">The type of validator used to validate the request.</typeparam>
        public class GenericValidation<TRequest, TValidator>
        {
            /// <summary>
            /// Validates a given request using the specified validator.
            /// </summary>
            /// <param name="request">The instance of the request to validate.</param>
            /// <returns>True if validation succeeds, otherwise false.</returns>
            /// <exception cref="ValidationException">Thrown when validation fails, containing detailed error messages.</exception>
            public bool Validate(TRequest request)
            {
                if (request != null)
                {
                    object[] args = new object[] { request };
                    Type type = typeof(TValidator);
                    MethodInfo? method = type.GetMethods().FirstOrDefault(x => x.Name.Equals("Validate", StringComparison.OrdinalIgnoreCase));
                    var instance = Activator.CreateInstance(type);
                    ValidationResult? result = method != null ? method.Invoke(instance, args) as ValidationResult : null;

                    if (result != null && result.IsValid == false)
                    {
                        var fields = result.Errors.Select(property => new
                        {
                            Name = property.PropertyName,
                            ValueProvided = property.AttemptedValue,
                            Error = property.ErrorMessage,
                        });

                        throw new ValidationException("Validation Error", fields);
                    }
                }

                return false;
            }
        }
    }
}
