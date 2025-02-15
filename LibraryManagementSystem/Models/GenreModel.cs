using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models
{
    /// <summary>
    /// Represents a book genre in the library system.
    /// Used to categorize books by their literary genre.
    /// </summary>
    public class GenreModel
    {
        /// <summary>
        /// The unique identifier for the genre.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The name of the genre. This field is required and must be unique.
        /// Maximum length is 100 characters.
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        /// <summary>
        /// Collection of books in this genre.
        /// Represents the one-to-many relationship between genres and books.
        /// </summary>
        public ICollection<BookModel> Books { get; set; } = new List<BookModel>();
    }
}
