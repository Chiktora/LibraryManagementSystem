using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models
{
    public class PublisherModel
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        // Навигационно свойство към книгите
        public ICollection<BookModel> Books { get; set; }
    }
}
