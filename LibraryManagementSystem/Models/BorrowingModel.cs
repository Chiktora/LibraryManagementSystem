using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace LibraryManagementSystem.Models
{
    public class BorrowingModel
    {
        public int Id { get; set; }

        [Required]
        public int BookId { get; set; }
        public BookModel? Book { get; set; } 

        [Required]
        public string? UserId { get; set; }
        // Свързване с ASP.NET Identity – таблицата AspNetUsers
        public IdentityUser? User { get; set; }

        [Required]
        public DateTime BorrowDate { get; set; }

        public DateTime? ReturnDate { get; set; }

        [Required]
        [MaxLength(20)]
        public string? Status { get; set; }
    }
}
