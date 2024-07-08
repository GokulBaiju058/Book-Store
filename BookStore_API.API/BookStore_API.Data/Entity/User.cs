using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookStore_API.Data.Entity
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        public string Username { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Phone { get; set; }

        public int Age { get; set; }

        public string? Address { get; set; }

        public string? Password { get; set; }

        public string? ZipCode { get; set; }

        public bool ? isActive { get; set; }

        [ForeignKey(nameof(Role))]
        public int RoleId { get; set; }

        public virtual Role Role { get; set; }
    }
}
