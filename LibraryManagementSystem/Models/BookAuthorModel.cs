namespace LibraryManagementSystem.Models
{
    /// <summary>
    /// Represents the many-to-many relationship between books and authors.
    /// This is a junction table that connects books with their authors.
    /// </summary>
    public class BookAuthorModel
    {
        /// <summary>
        /// Foreign key reference to the book.
        /// Forms part of the composite primary key.
        /// </summary>
        public int BookId { get; set; }

        /// <summary>
        /// Navigation property to the associated book.
        /// </summary>
        public BookModel? Book { get; set; }

        /// <summary>
        /// Foreign key reference to the author.
        /// Forms part of the composite primary key.
        /// </summary>
        public int AuthorId { get; set; }

        /// <summary>
        /// Navigation property to the associated author.
        /// </summary>
        public AuthorModel? Author { get; set; }
    }
}
