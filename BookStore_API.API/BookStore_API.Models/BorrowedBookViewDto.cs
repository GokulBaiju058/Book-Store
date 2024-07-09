
namespace BookStore_API.Models
{
    public class BorrowedBookViewDto
    {
        public int BorrowId { get; set; }

        public int BookId { get; set; }

        public string? BookName { get; set; }

        public string? Author { get; set; }

        public string? Genre { get; set; }
    }
}
