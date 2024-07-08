
namespace BookStore_API.Models
{
    /// <summary>
    /// Represents a generic response message returned by API endpoints.
    /// </summary>
    public abstract class GenericResponseMessage
    {
        public int StatusCode { get; set; } = 200;
        public bool Success { get; set; } = true;
        public string? Message { get; set; }
        public string? DetailMessage { get; set; }
    }
}
