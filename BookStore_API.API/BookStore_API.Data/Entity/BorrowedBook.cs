using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookStore_API.Data.Entity
{
    public class BorrowedBook
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(User))]
        public int UserId { get; set; }

        [ForeignKey(nameof(Book))]
        public int BookId { get; set; }

        [Required]
        public DateTime BorrowedDate { get; set; }

        public DateTime? ReturnedDate { get; set; }

        public string? Remarks { get; set; }

        public virtual User User { get; set; } = null!; // Ensure User is initialized as non-nullable

        public virtual Book Book { get; set; } = null!;
    }
}
