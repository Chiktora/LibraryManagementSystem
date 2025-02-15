using NUnit.Framework;
using LibraryManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System;

namespace LibraryManagementSystem_Tests
{
    [TestFixture]
    public class PublisherTests : TestBase
    {
        [Test]
        public void Create_ValidPublisher_ShouldAddSuccessfully()
        {
            // Arrange
            var publisher = new PublisherModel
            {
                Name = "Penguin Books"
            };

            // Act
            _context.Publishers.Add(publisher);
            _context.SaveChanges();

            // Assert
            var savedPublisher = _context.Publishers.Find(publisher.Id);
            Assert.That(savedPublisher, Is.Not.Null);
            Assert.That(savedPublisher.Name, Is.EqualTo("Penguin Books"));
        }

        [Test]
        public void Read_ExistingPublisher_ShouldReturnCorrectPublisher()
        {
            // Arrange
            var publisher = new PublisherModel
            {
                Name = "Penguin Books"
            };
            _context.Publishers.Add(publisher);
            _context.SaveChanges();

            // Act
            var retrievedPublisher = _context.Publishers.Find(publisher.Id);

            // Assert
            Assert.That(retrievedPublisher, Is.Not.Null);
            Assert.That(retrievedPublisher.Name, Is.EqualTo("Penguin Books"));
        }

        [Test]
        public void Update_ExistingPublisher_ShouldUpdateSuccessfully()
        {
            // Arrange
            var publisher = new PublisherModel
            {
                Name = "Penguin Books"
            };
            _context.Publishers.Add(publisher);
            _context.SaveChanges();

            // Act
            publisher.Name = "Penguin Random House";
            _context.SaveChanges();

            // Assert
            var updatedPublisher = _context.Publishers.Find(publisher.Id);
            Assert.That(updatedPublisher.Name, Is.EqualTo("Penguin Random House"));
        }

        [Test]
        public void Delete_ExistingPublisher_ShouldRemoveSuccessfully()
        {
            // Arrange
            var publisher = new PublisherModel
            {
                Name = "Penguin Books"
            };
            _context.Publishers.Add(publisher);
            _context.SaveChanges();

            // Act
            _context.Publishers.Remove(publisher);
            _context.SaveChanges();

            // Assert
            var deletedPublisher = _context.Publishers.Find(publisher.Id);
            Assert.That(deletedPublisher, Is.Null);
        }

        [Test]
        public void Read_PublisherWithBooks_ShouldIncludeBooks()
        {
            // Arrange
            var publisher = new PublisherModel
            {
                Name = "Penguin Books"
            };
            _context.Publishers.Add(publisher);

            var genre = new GenreModel { Name = "Fiction" };
            _context.Genres.Add(genre);
            _context.SaveChanges();

            var books = new[]
            {
                new BookModel
                {
                    Title = "Book 1",
                    ISBN = "1234567890",
                    GenreId = genre.Id,
                    PublisherId = publisher.Id,
                    Description = "Description 1"
                },
                new BookModel
                {
                    Title = "Book 2",
                    ISBN = "0987654321",
                    GenreId = genre.Id,
                    PublisherId = publisher.Id,
                    Description = "Description 2"
                }
            };
            _context.Books.AddRange(books);
            _context.SaveChanges();

            // Act
            var publisherWithBooks = _context.Publishers
                .Include(p => p.Books)
                .FirstOrDefault(p => p.Id == publisher.Id);

            // Assert
            Assert.That(publisherWithBooks, Is.Not.Null);
            Assert.That(publisherWithBooks.Books.Count, Is.EqualTo(2));
            Assert.That(publisherWithBooks.Books.Any(b => b.Title == "Book 1"), Is.True);
            Assert.That(publisherWithBooks.Books.Any(b => b.Title == "Book 2"), Is.True);
        }

        [Test]
        public void Delete_PublisherWithBooks_ShouldNotAllowDeletion()
        {
            // Arrange
            var publisher = new PublisherModel
            {
                Name = "Penguin Books"
            };
            _context.Publishers.Add(publisher);

            var genre = new GenreModel { Name = "Fiction" };
            _context.Genres.Add(genre);
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

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() =>
            {
                _context.Publishers.Remove(publisher);
                _context.SaveChanges();
            });

            // Verify the correct error message
            Assert.That(ex.Message, Does.Contain("The association between entity types 'PublisherModel' and 'BookModel' has been severed"));

            // Verify publisher and book still exist
            var publisherStillExists = _context.Publishers.Any(p => p.Id == publisher.Id);
            var bookStillExists = _context.Books.Any(b => b.Id == book.Id);
            Assert.That(publisherStillExists, Is.True);
            Assert.That(bookStillExists, Is.True);
        }
    }
} 