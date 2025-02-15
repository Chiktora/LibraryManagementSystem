using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Data
{
    /// <summary>
    /// The main database context for the Library Management System.
    /// Inherits from IdentityDbContext to include ASP.NET Identity tables.
    /// </summary>
    public class ApplicationDbContext : IdentityDbContext
    {
        /// <summary>
        /// Initializes a new instance of the ApplicationDbContext.
        /// </summary>
        /// <param name="options">The options to be used by the context.</param>
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// DbSet for accessing and managing books in the database.
        /// </summary>
        public DbSet<BookModel> Books { get; set; }

        /// <summary>
        /// DbSet for accessing and managing authors in the database.
        /// </summary>
        public DbSet<AuthorModel> Authors { get; set; }

        /// <summary>
        /// DbSet for accessing and managing book-author relationships in the database.
        /// </summary>
        public DbSet<BookAuthorModel> BookAuthors { get; set; }

        /// <summary>
        /// DbSet for accessing and managing publishers in the database.
        /// </summary>
        public DbSet<PublisherModel> Publishers { get; set; }

        /// <summary>
        /// DbSet for accessing and managing genres in the database.
        /// </summary>
        public DbSet<GenreModel> Genres { get; set; }

        /// <summary>
        /// DbSet for accessing and managing borrowing records in the database.
        /// </summary>
        public DbSet<BorrowingModel> Borrowings { get; set; }

        /// <summary>
        /// Configures the database model using the Fluent API.
        /// This method is called when the model for a derived context is being built.
        /// </summary>
        /// <param name="modelBuilder">The builder being used to construct the model.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Call base method to configure Identity tables
            base.OnModelCreating(modelBuilder);

            // Configure composite key for many-to-many relationship in BookAuthorModel
            modelBuilder.Entity<BookAuthorModel>()
                .HasKey(ba => new { ba.BookId, ba.AuthorId });

            // Configure BookModel entity
            modelBuilder.Entity<BookModel>(entity =>
            {
                entity.Property(b => b.Title)
                      .IsRequired()
                      .HasMaxLength(200);

                entity.Property(b => b.ISBN)
                      .HasMaxLength(20);

                // Configure delete behavior (Restrict - to prevent deletion of books when genre/publisher is deleted)
                entity.HasOne(b => b.Genre)
                      .WithMany(g => g.Books)
                      .HasForeignKey(b => b.GenreId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(b => b.Publisher)
                      .WithMany(p => p.Books)
                      .HasForeignKey(b => b.PublisherId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure AuthorModel entity
            modelBuilder.Entity<AuthorModel>(entity =>
            {
                entity.Property(a => a.FirstName)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(a => a.LastName)
                      .IsRequired()
                      .HasMaxLength(100);
            });

            // Configure PublisherModel entity
            modelBuilder.Entity<PublisherModel>(entity =>
            {
                entity.Property(p => p.Name)
                      .IsRequired()
                      .HasMaxLength(150);
            });

            // Configure GenreModel entity
            modelBuilder.Entity<GenreModel>(entity =>
            {
                entity.Property(g => g.Name)
                      .IsRequired()
                      .HasMaxLength(100);
            });

            // Configure BorrowingModel entity
            modelBuilder.Entity<BorrowingModel>(entity =>
            {
                entity.Property(b => b.Status)
                      .IsRequired()
                      .HasMaxLength(20);

                entity.Property(b => b.BorrowDate)
                      .IsRequired();
            });
        }
    }
}
