using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models
{
    /// <summary>
    /// Represents a book publisher in the library system.
    /// Publishers are organizations responsible for publishing books.
    /// </summary>
    public class PublisherModel
    {
        /// <summary>
        /// The unique identifier for the publisher.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The name of the publisher. This field is required and must be unique.
        /// Maximum length is 150 characters.
        /// </summary>
        [Required]
        [MaxLength(150)]
        public string Name { get; set; }

        /// <summary>
        /// Collection of books published by this publisher.
        /// Represents the one-to-many relationship between publishers and books.
        /// </summary>
        public ICollection<BookModel> Books { get; set; } = new List<BookModel>();
    }
}
