
namespace BookStore_API.Models
{
    /// <summary>
    /// Data transfer object (DTO) for registering a new user.
    /// </summary>
    public class RegisterUserDto
    {
        public int? Id { get; set; }

        public string? Username { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Phone { get; set; }

        public int Age { get; set; }

        public string? Address { get; set; }

        public string Password { get; set; }

        public string? ZipCode { get; set; }

    }
}
