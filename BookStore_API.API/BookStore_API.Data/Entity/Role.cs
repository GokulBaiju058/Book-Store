
using System.ComponentModel.DataAnnotations;

namespace BookStore_API.Data.Entity
{
    public class Role
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; } = null!;
    }
}
