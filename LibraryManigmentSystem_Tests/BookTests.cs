using NUnit.Framework;
using LibraryManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace LibraryManagementSystem_Tests
{
    [TestFixture]
    public class BookTests : TestBase
    {
        private GenreModel _testGenre;
        private PublisherModel _testPublisher;

        [SetUp]
        public void Setup()
        {
            // Create common test data
            _testGenre = new GenreModel { Name = "Fiction" };
            _testPublisher = new PublisherModel { Name = "Test Publisher" };
            _context.Genres.Add(_testGenre);
            _context.Publishers.Add(_testPublisher);
            _context.SaveChanges();
        }

        [Test]
        public void Create_ValidBook_ShouldAddSuccessfully()
        {
            // Arrange
            var book = new BookModel
            {
                Title = "Test Book",
                ISBN = "1234567890",
                GenreId = _testGenre.Id,
                PublisherId = _testPublisher.Id,
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
        public void Read_ExistingBook_ShouldReturnCorrectBook()
        {
            // Arrange
            var book = new BookModel
            {
                Title = "Test Book",
                ISBN = "1234567890",
                GenreId = _testGenre.Id,
                PublisherId = _testPublisher.Id,
                Description = "Test Description"
            };
            _context.Books.Add(book);
            _context.SaveChanges();

            // Act
            var retrievedBook = _context.Books
                .Include(b => b.Genre)
                .Include(b => b.Publisher)
                .FirstOrDefault(b => b.ISBN == "1234567890");

            // Assert
            Assert.That(retrievedBook, Is.Not.Null);
            Assert.That(retrievedBook.Title, Is.EqualTo("Test Book"));
            Assert.That(retrievedBook.ISBN, Is.EqualTo("1234567890"));
            Assert.That(retrievedBook.Genre.Name, Is.EqualTo("Fiction"));
            Assert.That(retrievedBook.Publisher.Name, Is.EqualTo("Test Publisher"));
        }

        [Test]
        public void Update_ExistingBook_ShouldUpdateSuccessfully()
        {
            // Arrange
            var book = new BookModel
            {
                Title = "Test Book",
                ISBN = "1234567890",
                GenreId = _testGenre.Id,
                PublisherId = _testPublisher.Id,
                Description = "Test Description"
            };
            _context.Books.Add(book);
            _context.SaveChanges();

            // Act
            book.Title = "Updated Title";
            book.Description = "Updated Description";
            _context.SaveChanges();

            // Assert
            var updatedBook = _context.Books.Find(book.Id);
            Assert.That(updatedBook.Title, Is.EqualTo("Updated Title"));
            Assert.That(updatedBook.Description, Is.EqualTo("Updated Description"));
        }

        [Test]
        public void Delete_ExistingBook_ShouldRemoveSuccessfully()
        {
            // Arrange
            var book = new BookModel
            {
                Title = "Test Book",
                ISBN = "1234567890",
                GenreId = _testGenre.Id,
                PublisherId = _testPublisher.Id,
                Description = "Test Description"
            };
            _context.Books.Add(book);
            _context.SaveChanges();

            // Act
            _context.Books.Remove(book);
            _context.SaveChanges();

            // Assert
            var deletedBook = _context.Books.Find(book.Id);
            Assert.That(deletedBook, Is.Null);
        }

        [Test]
        public void Create_BookWithAuthors_ShouldCreateRelationshipsSuccessfully()
        {
            // Arrange
            var author1 = new AuthorModel { FirstName = "John", LastName = "Doe" };
            var author2 = new AuthorModel { FirstName = "Jane", LastName = "Smith" };
            _context.Authors.AddRange(author1, author2);
            _context.SaveChanges();

            var book = new BookModel
            {
                Title = "Test Book",
                ISBN = "1234567890",
                GenreId = _testGenre.Id,
                PublisherId = _testPublisher.Id,
                Description = "Test Description",
                AuthorIds = new[] { author1.Id, author2.Id }
            };

            // Act
            _context.Books.Add(book);
            _context.SaveChanges();

            var bookAuthors = book.AuthorIds.Select(authorId => new BookAuthorModel
            {
                BookId = book.Id,
                AuthorId = authorId
            });
            _context.BookAuthors.AddRange(bookAuthors);
            _context.SaveChanges();

            // Assert
            var savedBook = _context.Books
                .Include(b => b.BookAuthors)
                    .ThenInclude(ba => ba.Author)
                .FirstOrDefault(b => b.Id == book.Id);

            Assert.That(savedBook.BookAuthors.Count, Is.EqualTo(2));
            Assert.That(savedBook.BookAuthors.Any(ba => ba.Author.FirstName == "John"), Is.True);
            Assert.That(savedBook.BookAuthors.Any(ba => ba.Author.FirstName == "Jane"), Is.True);
        }

        [Test]
        public void Update_BookAuthors_ShouldUpdateRelationshipsSuccessfully()
        {
            // Arrange
            var author1 = new AuthorModel { FirstName = "John", LastName = "Doe" };
            var author2 = new AuthorModel { FirstName = "Jane", LastName = "Smith" };
            var author3 = new AuthorModel { FirstName = "Bob", LastName = "Wilson" };
            _context.Authors.AddRange(author1, author2, author3);
            _context.SaveChanges();

            var book = new BookModel
            {
                Title = "Test Book",
                ISBN = "1234567890",
                GenreId = _testGenre.Id,
                PublisherId = _testPublisher.Id,
                Description = "Test Description",
                AuthorIds = new[] { author1.Id, author2.Id }
            };

            _context.Books.Add(book);
            _context.SaveChanges();

            var initialBookAuthors = book.AuthorIds.Select(authorId => new BookAuthorModel
            {
                BookId = book.Id,
                AuthorId = authorId
            });
            _context.BookAuthors.AddRange(initialBookAuthors);
            _context.SaveChanges();

            // Act - Update authors to only include author1 and author3
            var existingBookAuthors = _context.BookAuthors.Where(ba => ba.BookId == book.Id);
            _context.BookAuthors.RemoveRange(existingBookAuthors);

            var newBookAuthors = new[]
            {
                new BookAuthorModel { BookId = book.Id, AuthorId = author1.Id },
                new BookAuthorModel { BookId = book.Id, AuthorId = author3.Id }
            };
            _context.BookAuthors.AddRange(newBookAuthors);
            _context.SaveChanges();

            // Assert
            var updatedBook = _context.Books
                .Include(b => b.BookAuthors)
                    .ThenInclude(ba => ba.Author)
                .FirstOrDefault(b => b.Id == book.Id);

            Assert.That(updatedBook.BookAuthors.Count, Is.EqualTo(2));
            Assert.That(updatedBook.BookAuthors.Any(ba => ba.Author.FirstName == "John"), Is.True);
            Assert.That(updatedBook.BookAuthors.Any(ba => ba.Author.FirstName == "Bob"), Is.True);
            Assert.That(updatedBook.BookAuthors.Any(ba => ba.Author.FirstName == "Jane"), Is.False);
        }
    }
} 