
using BookStore_API.Data.Entity;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookStore_API.Models
{
    public class BorrowBookDto
    {
        public int UserId { get; set; }

        public int BookId { get; set; }

        public string? Remarks { get; set; }
    }
}
