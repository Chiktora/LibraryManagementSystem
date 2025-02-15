using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManagementSystem.Models
{
    /// <summary>
    /// Represents an author in the library system.
    /// Authors can write multiple books and books can have multiple authors.
    /// </summary>
    public class AuthorModel
    {
        /// <summary>
        /// The unique identifier for the author.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The author's first name. This field is required.
        /// Maximum length is 100 characters.
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; }

        /// <summary>
        /// The author's last name. This field is required.
        /// Maximum length is 100 characters.
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string LastName { get; set; }

        /// <summary>
        /// Gets the full name of the author by combining first and last name.
        /// This is a computed property.
        /// </summary>
        [NotMapped]
        public string FullName => $"{FirstName} {LastName}";

        /// <summary>
        /// Collection of book-author relationships.
        /// Represents the many-to-many relationship between authors and books.
        /// </summary>
        public ICollection<BookAuthorModel> BookAuthors { get; set; } = new List<BookAuthorModel>();
    }
}
