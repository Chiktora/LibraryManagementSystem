using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Data
{
    /// <summary>
    /// Responsible for seeding initial data into the database.
    /// This includes default users, roles, and sample data for testing.
    /// </summary>
    public static class DbSeeder
    {
        /// <summary>
        /// Seeds initial data into the database if it's empty.
        /// This includes creating admin users, roles, and sample data for books, authors, genres, and publishers.
        /// </summary>
        /// <param name="context">The database context to use for seeding.</param>
        /// <param name="userManager">The user manager for creating users.</param>
        /// <param name="roleManager">The role manager for creating roles.</param>
        /// <returns>A task representing the asynchronous seeding operation.</returns>
        public static async Task SeedData(ApplicationDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // Create roles if they don't exist
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            if (!await roleManager.RoleExistsAsync("User"))
            {
                await roleManager.CreateAsync(new IdentityRole("User"));
            }

            // Create admin user if it doesn't exist
            var adminEmail = "admin@library.com";
            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var adminUser = new IdentityUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, "Admin123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }

            // Seed initial data only if the database is empty
            if (!context.Genres.Any() && !context.Publishers.Any() && !context.Authors.Any())
            {
                // Add Genres
                var genres = new List<GenreModel>
                {
                    new GenreModel { Name = "Fiction" },
                    new GenreModel { Name = "Non-Fiction" },
                    new GenreModel { Name = "Science Fiction" },
                    new GenreModel { Name = "Mystery" },
                    new GenreModel { Name = "Romance" },
                    new GenreModel { Name = "Fantasy" }
                };
                await context.Genres.AddRangeAsync(genres);
                await context.SaveChangesAsync();

                // Add Publishers
                var publishers = new List<PublisherModel>
                {
                    new PublisherModel { Name = "Penguin Books" },
                    new PublisherModel { Name = "HarperCollins" },
                    new PublisherModel { Name = "Random House" },
                    new PublisherModel { Name = "Simon & Schuster" }
                };
                await context.Publishers.AddRangeAsync(publishers);
                await context.SaveChangesAsync();

                // Add Authors
                var authors = new List<AuthorModel>
                {
                    new AuthorModel { FirstName = "J.K.", LastName = "Rowling" },
                    new AuthorModel { FirstName = "George R.R.", LastName = "Martin" },
                    new AuthorModel { FirstName = "Stephen", LastName = "King" },
                    new AuthorModel { FirstName = "Agatha", LastName = "Christie" }
                };
                await context.Authors.AddRangeAsync(authors);
                await context.SaveChangesAsync();

                // Add sample books with relationships
                var books = new List<BookModel>
                {
                    new BookModel
                    {
                        Title = "Sample Book 1",
                        ISBN = "1234567890",
                        Description = "A sample book for testing",
                        GenreId = genres[0].Id,
                        PublisherId = publishers[0].Id,
                        PublishedDate = DateTime.Now.AddYears(-1)
                    },
                    new BookModel
                    {
                        Title = "Sample Book 2",
                        ISBN = "0987654321",
                        Description = "Another sample book for testing",
                        GenreId = genres[1].Id,
                        PublisherId = publishers[1].Id,
                        PublishedDate = DateTime.Now.AddMonths(-6)
                    }
                };
                await context.Books.AddRangeAsync(books);
                await context.SaveChangesAsync();

                // Add book-author relationships
                var bookAuthors = new List<BookAuthorModel>
                {
                    new BookAuthorModel { BookId = books[0].Id, AuthorId = authors[0].Id },
                    new BookAuthorModel { BookId = books[0].Id, AuthorId = authors[1].Id },
                    new BookAuthorModel { BookId = books[1].Id, AuthorId = authors[2].Id }
                };
                await context.BookAuthors.AddRangeAsync(bookAuthors);
                await context.SaveChangesAsync();
            }
        }
    }

    public enum Roles
    {
        Admin,
        User
    }
}
 