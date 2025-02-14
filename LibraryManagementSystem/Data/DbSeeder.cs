using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Data
{
    public static class DbSeeder
    {
        public static async Task SeedRolesAndAdminAsync(IServiceProvider service)
        {
            var userManager = service.GetService<UserManager<IdentityUser>>();
            var roleManager = service.GetService<RoleManager<IdentityRole>>();
            var context = service.GetService<ApplicationDbContext>();

            // Seed Roles
            await roleManager.CreateAsync(new IdentityRole(Roles.Admin.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.User.ToString()));

            // Create admin user
            var admin = new IdentityUser
            {
                UserName = "admin@library.com",
                Email = "admin@library.com",
                EmailConfirmed = true
            };

            var userExists = await userManager.FindByEmailAsync(admin.Email);
            if (userExists == null)
            {
                await userManager.CreateAsync(admin, "Admin@123");
                await userManager.AddToRoleAsync(admin, Roles.Admin.ToString());
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

                // Add Books with Authors
                var books = new List<BookModel>
                {
                    new BookModel
                    {
                        Title = "Harry Potter and the Philosopher's Stone",
                        ISBN = "978-0747532743",
                        GenreId = genres.First(g => g.Name == "Fantasy").Id,
                        PublisherId = publishers.First(p => p.Name == "Penguin Books").Id,
                        PublishedDate = new DateTime(1997, 6, 26),
                        Description = "The first book in the Harry Potter series."
                    },
                    new BookModel
                    {
                        Title = "A Game of Thrones",
                        ISBN = "978-0553103540",
                        GenreId = genres.First(g => g.Name == "Fantasy").Id,
                        PublisherId = publishers.First(p => p.Name == "Random House").Id,
                        PublishedDate = new DateTime(1996, 8, 1),
                        Description = "The first book in A Song of Ice and Fire series."
                    },
                    new BookModel
                    {
                        Title = "The Shining",
                        ISBN = "978-0385121675",
                        GenreId = genres.First(g => g.Name == "Fiction").Id,
                        PublisherId = publishers.First(p => p.Name == "Simon & Schuster").Id,
                        PublishedDate = new DateTime(1977, 1, 28),
                        Description = "A horror novel by Stephen King."
                    }
                };
                await context.Books.AddRangeAsync(books);
                await context.SaveChangesAsync();

                // Add Book-Author relationships
                var bookAuthors = new List<BookAuthorModel>
                {
                    new BookAuthorModel
                    {
                        BookId = books[0].Id,
                        AuthorId = authors.First(a => a.LastName == "Rowling").Id
                    },
                    new BookAuthorModel
                    {
                        BookId = books[1].Id,
                        AuthorId = authors.First(a => a.LastName == "Martin").Id
                    },
                    new BookAuthorModel
                    {
                        BookId = books[2].Id,
                        AuthorId = authors.First(a => a.LastName == "King").Id
                    }
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
 