
namespace BookStore_API.Models
{
    public class BookViewDto
    {
        public int Id { get; set; }

        public string? BookName { get; set; }

        public string? Author { get; set; }

        public string? Genre { get; set; }

        public bool? isAvailable { get; set; }
    }
}
