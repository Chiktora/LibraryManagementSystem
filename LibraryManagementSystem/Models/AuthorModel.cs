using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManagementSystem.Models
{
    public class AuthorModel
    {
        public int Id { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [NotMapped]
        public string FullName => $"{FirstName} {LastName}";

        // Навигационно свойство за many-to-many връзката с книгите
        public ICollection<BookAuthorModel> BookAuthors { get; set; } = new List<BookAuthorModel>();
    }
}
