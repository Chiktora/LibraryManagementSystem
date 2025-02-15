using NUnit.Framework;
using LibraryManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System;

namespace LibraryManagementSystem_Tests
{
    [TestFixture]
    public class GenreTests : TestBase
    {
        [Test]
        public void Create_ValidGenre_ShouldAddSuccessfully()
        {
            // Arrange
            var genre = new GenreModel
            {
                Name = "Science Fiction"
            };

            // Act
            _context.Genres.Add(genre);
            _context.SaveChanges();

            // Assert
            var savedGenre = _context.Genres.Find(genre.Id);
            Assert.That(savedGenre, Is.Not.Null);
            Assert.That(savedGenre.Name, Is.EqualTo("Science Fiction"));
        }

        [Test]
        public void Read_ExistingGenre_ShouldReturnCorrectGenre()
        {
            // Arrange
            var genre = new GenreModel
            {
                Name = "Science Fiction"
            };
            _context.Genres.Add(genre);
            _context.SaveChanges();

            // Act
            var retrievedGenre = _context.Genres.Find(genre.Id);

            // Assert
            Assert.That(retrievedGenre, Is.Not.Null);
            Assert.That(retrievedGenre.Name, Is.EqualTo("Science Fiction"));
        }

        [Test]
        public void Update_ExistingGenre_ShouldUpdateSuccessfully()
        {
            // Arrange
            var genre = new GenreModel
            {
                Name = "Science Fiction"
            };
            _context.Genres.Add(genre);
            _context.SaveChanges();

            // Act
            genre.Name = "Sci-Fi";
            _context.SaveChanges();

            // Assert
            var updatedGenre = _context.Genres.Find(genre.Id);
            Assert.That(updatedGenre.Name, Is.EqualTo("Sci-Fi"));
        }

        [Test]
        public void Delete_ExistingGenre_ShouldRemoveSuccessfully()
        {
            // Arrange
            var genre = new GenreModel
            {
                Name = "Science Fiction"
            };
            _context.Genres.Add(genre);
            _context.SaveChanges();

            // Act
            _context.Genres.Remove(genre);
            _context.SaveChanges();

            // Assert
            var deletedGenre = _context.Genres.Find(genre.Id);
            Assert.That(deletedGenre, Is.Null);
        }

        [Test]
        public void Read_GenreWithBooks_ShouldIncludeBooks()
        {
            // Arrange
            var genre = new GenreModel
            {
                Name = "Science Fiction"
            };
            _context.Genres.Add(genre);

            var publisher = new PublisherModel { Name = "Test Publisher" };
            _context.Publishers.Add(publisher);
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
            var genreWithBooks = _context.Genres
                .Include(g => g.Books)
                .FirstOrDefault(g => g.Id == genre.Id);

            // Assert
            Assert.That(genreWithBooks, Is.Not.Null);
            Assert.That(genreWithBooks.Books.Count, Is.EqualTo(2));
            Assert.That(genreWithBooks.Books.Any(b => b.Title == "Book 1"), Is.True);
            Assert.That(genreWithBooks.Books.Any(b => b.Title == "Book 2"), Is.True);
        }

        [Test]
        public void Delete_GenreWithBooks_ShouldNotAllowDeletion()
        {
            // Arrange
            var genre = new GenreModel
            {
                Name = "Science Fiction"
            };
            _context.Genres.Add(genre);

            var publisher = new PublisherModel { Name = "Test Publisher" };
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

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() =>
            {
                _context.Genres.Remove(genre);
                _context.SaveChanges();
            });

            // Verify the correct error message
            Assert.That(ex.Message, Does.Contain("The association between entity types 'GenreModel' and 'BookModel' has been severed"));

            // Verify genre and book still exist
            var genreStillExists = _context.Genres.Any(g => g.Id == genre.Id);
            var bookStillExists = _context.Books.Any(b => b.Id == book.Id);
            Assert.That(genreStillExists, Is.True);
            Assert.That(bookStillExists, Is.True);
        }
    }
} 