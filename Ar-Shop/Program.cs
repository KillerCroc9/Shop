using Ar_Shop.Data;
using Ar_Shop.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Ar_Shop
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddRoles<IdentityRole>() // Add support for roles
                .AddEntityFrameworkStores<ApplicationDbContext>();

            builder.Services.AddControllersWithViews();
            // Add authorization policy for administrators
            builder.Services.AddSession();

            var app = builder.Build();
            using var scope = app.Services.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Ensure the "Admin" role exists
            if (!roleManager.RoleExistsAsync("Admin").Result)
            {
                IdentityRole role = new IdentityRole
                {
                    Name = "Admin"
                };
                IdentityResult roleResult = roleManager.CreateAsync(role).Result;
            }


            // Ensure the "Administrator" role exists
            app.UseSession();



            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }



            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
            name: "allProducts",
            pattern: "Products/AllProducts",
            defaults: new { controller = "Products", action = "AllProducts" });



            app.MapControllerRoute(
            name: "cart",
            pattern: "cart/{action=Index}/{id?}",
            defaults: new { controller = "Cart" });


            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            // In Startup.cs, configure the route to the AssignAdminRole action
            app.MapControllerRoute(
                name: "AssignAdminRole",
                pattern: "Admin/AssignAdminRole/{userId}",
                defaults: new { controller = "Admin", action = "AssignAdminRole" });

// Then, in your view or another controller, generate a link with the userId as a query parameter

            app.MapRazorPages();

            app.Run();
        }
    }
}