using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace LibraryManagementSystem.Models
{
    /// <summary>
    /// Represents a book borrowing record in the library system.
    /// Tracks when books are borrowed and returned by users.
    /// </summary>
    public class BorrowingModel
    {
        /// <summary>
        /// The unique identifier for the borrowing record.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Foreign key reference to the borrowed book.
        /// This field is required.
        /// </summary>
        [Required]
        public int BookId { get; set; }

        /// <summary>
        /// Navigation property to the borrowed book.
        /// </summary>
        public BookModel? Book { get; set; }

        /// <summary>
        /// Foreign key reference to the user who borrowed the book.
        /// Links to ASP.NET Identity Users table.
        /// This field is required.
        /// </summary>
        [Required]
        public string? UserId { get; set; }

        /// <summary>
        /// Navigation property to the user who borrowed the book.
        /// Uses ASP.NET Identity User model.
        /// </summary>
        public IdentityUser? User { get; set; }

        /// <summary>
        /// The date when the book was borrowed.
        /// This field is required.
        /// </summary>
        [Required]
        public DateTime BorrowDate { get; set; }

        /// <summary>
        /// The date when the book was returned.
        /// Null if the book hasn't been returned yet.
        /// </summary>
        public DateTime? ReturnDate { get; set; }

        /// <summary>
        /// The current status of the borrowing (e.g., "Pending", "Active", "Returned").
        /// Maximum length is 20 characters.
        /// This field is required.
        /// </summary>
        [Required]
        [MaxLength(20)]
        public string? Status { get; set; }
    }
}
