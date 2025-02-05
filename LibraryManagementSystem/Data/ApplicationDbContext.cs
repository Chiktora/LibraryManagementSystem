using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<BookModel> Books { get; set; }
        public DbSet<AuthorModel> Authors { get; set; }
        public DbSet<BookAuthorModel> BookAuthors { get; set; }
        public DbSet<PublisherModel> Publishers { get; set; }
        public DbSet<GenreModel> Genres { get; set; }
        public DbSet<BorrowingModel> Borrowings { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Извикваме базовия метод, за да се конфигурират Identity таблиците
            base.OnModelCreating(modelBuilder);

            // Конфигуриране на composite ключ за many-to-many връзката в таблицата BookAuthorModel
            modelBuilder.Entity<BookAuthorModel>()
                .HasKey(ba => new { ba.BookId, ba.AuthorId });

            // Конфигурация за BookModel
            modelBuilder.Entity<BookModel>(entity =>
            {
                entity.Property(b => b.Title)
                      .IsRequired()
                      .HasMaxLength(200);

                entity.Property(b => b.ISBN)
                      .HasMaxLength(20);

                // Задаване на поведение при изтриване (Restrict – за да не се изтриват книгите при изтриване на жанр/издателство)
                entity.HasOne(b => b.Genre)
                      .WithMany(g => g.Books)
                      .HasForeignKey(b => b.GenreId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(b => b.Publisher)
                      .WithMany(p => p.Books)
                      .HasForeignKey(b => b.PublisherId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Конфигурация за AuthorModel
            modelBuilder.Entity<AuthorModel>(entity =>
            {
                entity.Property(a => a.FirstName)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(a => a.LastName)
                      .IsRequired()
                      .HasMaxLength(100);
            });

            // Конфигурация за PublisherModel
            modelBuilder.Entity<PublisherModel>(entity =>
            {
                entity.Property(p => p.Name)
                      .IsRequired()
                      .HasMaxLength(150);
            });

            // Конфигурация за GenreModel
            modelBuilder.Entity<GenreModel>(entity =>
            {
                entity.Property(g => g.Name)
                      .IsRequired()
                      .HasMaxLength(100);
            });

            // Конфигурация за BorrowingModel
            modelBuilder.Entity<BorrowingModel>(entity =>
            {
                entity.Property(b => b.Status)
                      .IsRequired()
                      .HasMaxLength(20);

                entity.Property(b => b.BorrowDate)
                      .IsRequired();

                // Можете да добавите и други настройки, напр. индекс върху UserId, ако е необходимо:
                // entity.HasIndex(b => b.UserId);
            });

            // Допълнителни конфигурации може да се добавят тук, ако има нужда от по-специфично поведение.
        }

    }
    
}
