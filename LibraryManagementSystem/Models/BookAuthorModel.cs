namespace LibraryManagementSystem.Models
{
    public class BookAuthorModel
    {
        public int BookId { get; set; }
        public BookModel? Book { get; set; }

        public int AuthorId { get; set; }
        public AuthorModel? Author { get; set; }
    }
}
