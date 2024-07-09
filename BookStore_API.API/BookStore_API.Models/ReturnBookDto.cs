
namespace BookStore_API.Models
{
    public class ReturnBookDto
    {
        public int BorrowId { get; set; }

        public int UserId { get; set; }

        public int BookId { get; set; }

        public string? Remarks { get; set; }
    }
}
