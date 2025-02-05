using LibraryManagementSystem.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // 1) Connection string for ApplicationDbContext
            var connectionString = Configuration.ConnectionString
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            // 2) Register ApplicationDbContext + EF
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            // 3) Configure Identity (with Roles)
            builder.Services.AddDefaultIdentity<IdentityUser>(options =>
            {
                // Пример: RequireConfirmedAccount = true означава, че потребителят трябва да потвърди имейла си
                options.SignIn.RequireConfirmedAccount = true;
            })
            .AddRoles<IdentityRole>() // <-- Добавяме роля
            .AddEntityFrameworkStores<ApplicationDbContext>();

            // 4) Add MVC
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // 5) Configure environment-specific settings
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            // 6) Middlewares
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            // 7) Optional: Seed roles/admin user here
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                    var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
                    SeedRolesAndAdmin(roleManager, userManager);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error during seeding: " + ex.Message);
                }
            }

            // 8) Map routes
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.MapRazorPages();

            app.Run();
        }

        private static void SeedRolesAndAdmin(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
        {
            // Тъй като Main е void, ще извикаме асинхронните методи със .GetAwaiter().GetResult()
            var rolesTask = SeedRoles(roleManager);
            rolesTask.GetAwaiter().GetResult();

            var adminTask = SeedAdminUser(userManager);
            adminTask.GetAwaiter().GetResult();
        }

        private static async Task SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            string[] roles = { "Admin", "User" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }

        private static async Task SeedAdminUser(UserManager<IdentityUser> userManager)
        {
            string adminEmail = "admin@library.com";
            string adminPassword = "Admin123!";

            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new IdentityUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(adminUser, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        Console.WriteLine("Error creating admin: " + error.Description);
                    }
                }
            }
        }
    }
}
