using BookStore_API.Data.Entity;
using Microsoft.EntityFrameworkCore;

namespace BookStore_API.Data
{
    /// <summary>
    /// Represents the database context for the BookStore API, providing access to entities and defining their relationships.
    /// </summary>
    public class BookStore_APIContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<BorrowedBook> BorrowedBooks { get; set; }

        public BookStore_APIContext(DbContextOptions<BookStore_APIContext> options) : base(options)
        {
        }

        /// <summary>
        /// Configures entity relationships and initializes data seeding for the database context.
        /// </summary>
        /// <param name="modelBuilder">The model builder instance used to define entity configurations.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Define relationships
            modelBuilder.Entity<BorrowedBook>()
                .HasOne(bb => bb.User)
                .WithMany()
                .HasForeignKey(bb => bb.UserId)
                .OnDelete(DeleteBehavior.Restrict); // Or DeleteBehavior.Cascade if you want to cascade delete

            modelBuilder.Entity<BorrowedBook>()
                .HasOne(bb => bb.Book)
                .WithMany()
                .HasForeignKey(bb => bb.BookId)
                .OnDelete(DeleteBehavior.Restrict); // Or DeleteBehavior.Cascade if you want to cascade delete

            // Indexes
            modelBuilder.Entity<Book>()
                .HasIndex(b => b.BookName);

            modelBuilder.Entity<Book>()
                .HasIndex(b => b.Author);

            modelBuilder.Entity<Book>()
                .HasIndex(b => b.Genre);

            //Seed Initial Data
            modelBuilder.Entity<Role>().HasData(
                new Role
                {
                    Id = 1,
                    Name = "User"
                },
                new Role
                {
                    Id = 2,
                    Name = "LibraryAdmin"
                }
                );

            modelBuilder.Entity<Book>().HasData(
                new Book
                {
                    Id = 1,
                    BookName = "To Kill a Mockingbird",
                    Author = "Harper Lee",
                    Genre ="Drama",
                    IsActive = true,
                    CurrentQty = 1,
                    TotalQty = 1,
                },
                new Book
                {
                    Id = 2,
                    BookName = "1984",
                    Author = "George Orwell",
                    Genre = "Drama",
                    IsActive = true,
                    CurrentQty = 2,
                    TotalQty = 2,
                },
                new Book
                {
                    Id = 3,
                    BookName = "Pride and Prejudice",
                    Author = "Jane Austen",
                    Genre = "Drama",
                    IsActive= true,
                    CurrentQty = 1,
                    TotalQty = 1,
                }
                // Add more books as needed
            );

            base.OnModelCreating(modelBuilder);
        }
    }
}
