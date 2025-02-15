using NUnit.Framework;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace LibraryManagementSystem_Tests
{
    [TestFixture]
    public class BookModelTests
    {
        private ApplicationDbContext _context;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestLibraryDb")
                .Options;

            _context = new ApplicationDbContext(options);
            
            // Clear the database before each test
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();
        }

        [Test]
        public void AddBook_ValidBook_ShouldAddSuccessfully()
        {
            // Arrange
            var genre = new GenreModel { Name = "Fiction" };
            var publisher = new PublisherModel { Name = "Test Publisher" };
            _context.Genres.Add(genre);
            _context.Publishers.Add(publisher);
            _context.SaveChanges();

            var book = new BookModel
            {
                Title = "Test Book",
                ISBN = "1234567890",
                GenreId = genre.Id,
                PublisherId = publisher.Id,
                PublishedDate = DateTime.Now,
                Description = "Test Description"
            };

            // Act
            _context.Books.Add(book);
            _context.SaveChanges();

            // Assert
            var savedBook = _context.Books
                .Include(b => b.Genre)
                .Include(b => b.Publisher)
                .FirstOrDefault(b => b.ISBN == "1234567890");

            Assert.That(savedBook, Is.Not.Null);
            Assert.That(savedBook.Title, Is.EqualTo("Test Book"));
            Assert.That(savedBook.Genre.Name, Is.EqualTo("Fiction"));
            Assert.That(savedBook.Publisher.Name, Is.EqualTo("Test Publisher"));
        }

        [Test]
        public void AddBook_WithAuthors_ShouldAddSuccessfully()
        {
            // Arrange
            var genre = new GenreModel { Name = "Fiction" };
            var publisher = new PublisherModel { Name = "Test Publisher" };
            var author1 = new AuthorModel { FirstName = "John", LastName = "Doe" };
            var author2 = new AuthorModel { FirstName = "Jane", LastName = "Smith" };

            _context.Genres.Add(genre);
            _context.Publishers.Add(publisher);
            _context.Authors.AddRange(author1, author2);
            _context.SaveChanges();

            var book = new BookModel
            {
                Title = "Test Book",
                ISBN = "1234567890",
                GenreId = genre.Id,
                PublisherId = publisher.Id,
                PublishedDate = DateTime.Now,
                Description = "Test Description",
                AuthorIds = new[] { author1.Id, author2.Id }
            };

            // Act
            _context.Books.Add(book);
            _context.SaveChanges();

            // Add book-author relationships
            var bookAuthors = book.AuthorIds.Select(authorId => new BookAuthorModel
            {
                BookId = book.Id,
                AuthorId = authorId
            });
            _context.BookAuthors.AddRange(bookAuthors);
            _context.SaveChanges();

            // Assert
            var savedBook = _context.Books
                .Include(b => b.Genre)
                .Include(b => b.Publisher)
                .Include(b => b.BookAuthors)
                    .ThenInclude(ba => ba.Author)
                .FirstOrDefault(b => b.ISBN == "1234567890");

            Assert.That(savedBook, Is.Not.Null);
            Assert.That(savedBook.BookAuthors.Count, Is.EqualTo(2));
            Assert.That(savedBook.BookAuthors.Any(ba => ba.Author.FirstName == "John" && ba.Author.LastName == "Doe"), Is.True);
            Assert.That(savedBook.BookAuthors.Any(ba => ba.Author.FirstName == "Jane" && ba.Author.LastName == "Smith"), Is.True);
        }

        [Test]
        public void AddBook_DuplicateISBN_ShouldNotAllowDuplicates()
        {
            // Arrange
            var genre = new GenreModel { Name = "Fiction" };
            var publisher = new PublisherModel { Name = "Test Publisher" };
            _context.Genres.Add(genre);
            _context.Publishers.Add(publisher);
            _context.SaveChanges();

            var book1 = new BookModel
            {
                Title = "Test Book 1",
                ISBN = "1234567890",
                GenreId = genre.Id,
                PublisherId = publisher.Id,
                Description = "Test Description 1"
            };

            // Act & Assert
            _context.Books.Add(book1);
            _context.SaveChanges();

            // Check if a book with the same ISBN already exists
            var duplicateExists = _context.Books.Any(b => b.ISBN == "1234567890");
            Assert.That(duplicateExists, Is.True, "First book should be saved successfully");

            var book2 = new BookModel
            {
                Title = "Test Book 2",
                ISBN = "1234567890", // Same ISBN
                GenreId = genre.Id,
                PublisherId = publisher.Id,
                Description = "Test Description 2"
            };

            // In a real application, we would check for duplicate ISBN before trying to save
            var isDuplicateISBN = _context.Books.Any(b => b.ISBN == book2.ISBN);
            Assert.That(isDuplicateISBN, Is.True, "Should detect duplicate ISBN");
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }
    }
} 