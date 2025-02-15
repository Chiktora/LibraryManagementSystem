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
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
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

            // 7) Map routes
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.MapRazorPages();

            // Seed Database
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<ApplicationDbContext>();
                var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

                DbSeeder.SeedData(context, userManager, roleManager).GetAwaiter().GetResult();
            }

            app.Run();
        }
    }
}
