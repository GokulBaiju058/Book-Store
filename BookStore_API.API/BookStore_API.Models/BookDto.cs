
namespace BookStore_API.Models
{
    public class BookDto
    {
        public int Id { get; set; }

        public string? BookName { get; set; }

        public string? Author { get; set; }

        public string? Genre { get; set; }

        public bool? isActive { get; set; }

        public int? CurrentQty { get; set; }

        public int? TotalQty { get; set; }
    }
}
