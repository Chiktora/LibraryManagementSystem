using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManagementSystem.Models
{
    /// <summary>
    /// Represents a book in the library system.
    /// This is the main entity for storing book information.
    /// </summary>
    public class BookModel
    {
        /// <summary>
        /// The unique identifier for the book.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The title of the book. This field is required.
        /// </summary>
        [Required]
        public string Title { get; set; }

        /// <summary>
        /// The International Standard Book Number (ISBN).
        /// Must be unique within the system.
        /// </summary>
        public string ISBN { get; set; }

        /// <summary>
        /// Foreign key reference to the book's genre.
        /// </summary>
        public int GenreId { get; set; }

        /// <summary>
        /// Navigation property to the book's genre.
        /// </summary>
        public GenreModel? Genre { get; set; }

        /// <summary>
        /// Foreign key reference to the book's publisher.
        /// </summary>
        public int PublisherId { get; set; }

        /// <summary>
        /// Navigation property to the book's publisher.
        /// </summary>
        public PublisherModel? Publisher { get; set; }

        /// <summary>
        /// The date when the book was published.
        /// </summary>
        public DateTime? PublishedDate { get; set; }

        /// <summary>
        /// A detailed description of the book.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Collection of book-author relationships.
        /// Represents the many-to-many relationship between books and authors.
        /// </summary>
        public ICollection<BookAuthorModel> BookAuthors { get; set; } = new List<BookAuthorModel>();

        /// <summary>
        /// Collection of borrowing records for this book.
        /// </summary>
        public ICollection<BorrowingModel> Borrowings { get; set; } = new List<BorrowingModel>();

        /// <summary>
        /// Array of author IDs used for binding in forms.
        /// Not mapped to the database.
        /// </summary>
        [NotMapped]
        public int[] AuthorIds { get; set; }
    }
}
