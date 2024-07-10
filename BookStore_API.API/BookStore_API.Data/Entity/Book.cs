
using System.ComponentModel.DataAnnotations;

namespace BookStore_API.Data.Entity
{
    public class Book
    {
        [Key]
        public int Id { get; set; }

        public string? BookName { get; set; }

        public string? Author { get; set; }

        public string? Genre { get; set; }

        public bool? IsActive { get; set; }

        public int? CurrentQty { get; set; }

        public int? TotalQty { get; set; }
    }
}
