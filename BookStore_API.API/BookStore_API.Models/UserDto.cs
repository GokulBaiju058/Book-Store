using BookStore_API.Data.Entity;

namespace BookStore_API.Models
{
    public class UserDto
    {
        public string? Username { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Phone { get; set; }

        public int? Age { get; set; }

        public string? Address { get; set; }

        public string? ZipCode { get; set; }

        public bool? isActive { get; set; }

    }
}
