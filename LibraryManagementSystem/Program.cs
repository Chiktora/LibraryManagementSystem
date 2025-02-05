using LibraryManagementSystem.Data;  // ?? ?????? ?? LibraryDbContext
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ?????? ?? connection string ?? ?????????????? (appsettings.json)
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            // ???????????? ?? LibraryDbContext ? MSSQL Server
            builder.Services.AddDbContext<LibraryDbContext>(options =>
                options.UseSqlServer(connectionString));

            // ???????????? ?? ASP.NET Identity ? ?????????? ?? LibraryDbContext
            builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
            {
                // ??? ???? ?? ??????????????? ??????????? ?? Identity (????????, ?????????? ?? ??????)
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
            })
            .AddEntityFrameworkStores<LibraryDbContext>()
            .AddDefaultTokenProviders();

            // ???????????? ?? ???????????? ? ??????? (MVC)
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // ????????????? ?? HTTP request pipeline
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // ?? ???????????? HSTS ?????????? ? 30 ???
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            // ?????? ?? ???????? UseAuthentication(), ?? ?? ?? ????????? ??????????????, ????? UseAuthorization()
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
