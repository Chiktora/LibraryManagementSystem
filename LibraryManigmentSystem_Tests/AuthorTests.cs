using NUnit.Framework;
using LibraryManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace LibraryManagementSystem_Tests
{
    [TestFixture]
    public class AuthorTests : TestBase
    {
        [Test]
        public void Create_ValidAuthor_ShouldAddSuccessfully()
        {
            // Arrange
            var author = new AuthorModel
            {
                FirstName = "John",
                LastName = "Doe"
            };

            // Act
            _context.Authors.Add(author);
            _context.SaveChanges();

            // Assert
            var savedAuthor = _context.Authors.Find(author.Id);
            Assert.That(savedAuthor, Is.Not.Null);
            Assert.That(savedAuthor.FirstName, Is.EqualTo("John"));
            Assert.That(savedAuthor.LastName, Is.EqualTo("Doe"));
            Assert.That(savedAuthor.FullName, Is.EqualTo("John Doe"));
        }

        [Test]
        public void Read_ExistingAuthor_ShouldReturnCorrectAuthor()
        {
            // Arrange
            var author = new AuthorModel
            {
                FirstName = "John",
                LastName = "Doe"
            };
            _context.Authors.Add(author);
            _context.SaveChanges();

            // Act
            var retrievedAuthor = _context.Authors.Find(author.Id);

            // Assert
            Assert.That(retrievedAuthor, Is.Not.Null);
            Assert.That(retrievedAuthor.FirstName, Is.EqualTo("John"));
            Assert.That(retrievedAuthor.LastName, Is.EqualTo("Doe"));
        }

        [Test]
        public void Update_ExistingAuthor_ShouldUpdateSuccessfully()
        {
            // Arrange
            var author = new AuthorModel
            {
                FirstName = "John",
                LastName = "Doe"
            };
            _context.Authors.Add(author);
            _context.SaveChanges();

            // Act
            author.FirstName = "Jane";
            author.LastName = "Smith";
            _context.SaveChanges();

            // Assert
            var updatedAuthor = _context.Authors.Find(author.Id);
            Assert.That(updatedAuthor.FirstName, Is.EqualTo("Jane"));
            Assert.That(updatedAuthor.LastName, Is.EqualTo("Smith"));
            Assert.That(updatedAuthor.FullName, Is.EqualTo("Jane Smith"));
        }

        [Test]
        public void Delete_ExistingAuthor_ShouldRemoveSuccessfully()
        {
            // Arrange
            var author = new AuthorModel
            {
                FirstName = "John",
                LastName = "Doe"
            };
            _context.Authors.Add(author);
            _context.SaveChanges();

            // Act
            _context.Authors.Remove(author);
            _context.SaveChanges();

            // Assert
            var deletedAuthor = _context.Authors.Find(author.Id);
            Assert.That(deletedAuthor, Is.Null);
        }

        [Test]
        public void Read_AuthorWithBooks_ShouldIncludeBooks()
        {
            // Arrange
            var author = new AuthorModel
            {
                FirstName = "John",
                LastName = "Doe"
            };
            _context.Authors.Add(author);

            var genre = new GenreModel { Name = "Fiction" };
            var publisher = new PublisherModel { Name = "Test Publisher" };
            _context.Genres.Add(genre);
            _context.Publishers.Add(publisher);
            _context.SaveChanges();

            var book1 = new BookModel
            {
                Title = "Book 1",
                ISBN = "1234567890",
                GenreId = genre.Id,
                PublisherId = publisher.Id,
                Description = "Description 1"
            };
            var book2 = new BookModel
            {
                Title = "Book 2",
                ISBN = "0987654321",
                GenreId = genre.Id,
                PublisherId = publisher.Id,
                Description = "Description 2"
            };
            _context.Books.AddRange(book1, book2);
            _context.SaveChanges();

            var bookAuthors = new[]
            {
                new BookAuthorModel { BookId = book1.Id, AuthorId = author.Id },
                new BookAuthorModel { BookId = book2.Id, AuthorId = author.Id }
            };
            _context.BookAuthors.AddRange(bookAuthors);
            _context.SaveChanges();

            // Act
            var authorWithBooks = _context.Authors
                .Include(a => a.BookAuthors)
                    .ThenInclude(ba => ba.Book)
                .FirstOrDefault(a => a.Id == author.Id);

            // Assert
            Assert.That(authorWithBooks, Is.Not.Null);
            Assert.That(authorWithBooks.BookAuthors.Count, Is.EqualTo(2));
            Assert.That(authorWithBooks.BookAuthors.Any(ba => ba.Book.Title == "Book 1"), Is.True);
            Assert.That(authorWithBooks.BookAuthors.Any(ba => ba.Book.Title == "Book 2"), Is.True);
        }

        [Test]
        public void Delete_AuthorWithBooks_ShouldRemoveRelationships()
        {
            // Arrange
            var author = new AuthorModel
            {
                FirstName = "John",
                LastName = "Doe"
            };
            _context.Authors.Add(author);

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
                Description = "Test Description"
            };
            _context.Books.Add(book);
            _context.SaveChanges();

            var bookAuthor = new BookAuthorModel { BookId = book.Id, AuthorId = author.Id };
            _context.BookAuthors.Add(bookAuthor);
            _context.SaveChanges();

            // Act
            _context.Authors.Remove(author);
            _context.SaveChanges();

            // Assert
            var deletedAuthor = _context.Authors.Find(author.Id);
            Assert.That(deletedAuthor, Is.Null);

            var remainingRelationships = _context.BookAuthors.Any(ba => ba.AuthorId == author.Id);
            Assert.That(remainingRelationships, Is.False);

            // Verify book still exists
            var bookStillExists = _context.Books.Any(b => b.Id == book.Id);
            Assert.That(bookStillExists, Is.True);
        }
    }
} 