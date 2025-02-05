using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models
{
    public class BookModel
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        public string ISBN { get; set; }

        // Връзка към жанра (Foreign Key)
        public int GenreId { get; set; }
        public GenreModel Genre { get; set; }

        // Връзка към издателството (Foreign Key)
        public int PublisherId { get; set; }
        public PublisherModel Publisher { get; set; }

        public DateTime? PublishedDate { get; set; }

        public string Description { get; set; }

        // Навигационно свойство за many-to-many връзката с авторите
        public ICollection<BookAuthorModel> BookAuthors { get; set; }

        // Навигационно свойство за заемания
        public ICollection<BorrowingModel> Borrowings { get; set; }
    }
}
