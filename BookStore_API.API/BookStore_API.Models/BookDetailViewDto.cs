using System;
using System.Collections.Generic;

namespace BookStore_API.Models
{
    public class BookDetailViewDto
    {
        public int Id { get; set; }

        public string? BookName { get; set; }

        public string? Author { get; set; }

        public string? Genre { get; set; }

        public bool? isActive { get; set; }

        public int? CurrentQty { get; set; }

        public int? TotalQty { get; set; }

        public List<BorrowerDto>? Borrowers { get; set; }
    }

    public class BorrowerDto
    {
        public string? Username { get; set; }

        public DateTime? BorrowedDate { get; set; }
    }
}
