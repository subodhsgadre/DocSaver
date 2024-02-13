using DocSaver.DataAccess;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DocSaver
{
    public class Program
    {
        const string adminRole = "Admin";
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DocSaverConnectionString"));
            });
            builder.Services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.AccessDeniedPath = "/Account/Login";
                options.LoginPath = "/Account/Login";
                options.LogoutPath = "/Account/Login";
                options.Cookie.Name = "DocSaverApplication";
                options.ExpireTimeSpan = TimeSpan.FromDays(10);
                options.SlidingExpiration = true;
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            AddAdminRole(app);

            app.Run();       
        }

        private static void AddAdminRole(WebApplication web)
        {
            var scope = web.Services.CreateScope();

            //Resolve ASP .NET Core Identity with DI help
            var userManager = (UserManager<IdentityUser>)scope.ServiceProvider.GetService(typeof(UserManager<IdentityUser>));
            var roleManager = (RoleManager<IdentityRole>)scope.ServiceProvider.GetService(typeof(RoleManager<IdentityRole>));

            Task<IdentityResult> roleResult;
            string email = "sg_admin@mydomain.com";
            string username = "sg_admin";

            string[] roleNames = { adminRole, "User" };

            foreach (var roleName in roleNames)
            {
                //Check that there is an Administrator role and create if not
                Task<bool> hasAdminRole = roleManager.RoleExistsAsync(roleName);
                hasAdminRole.Wait();

                if (!hasAdminRole.Result)
                {
                    roleResult = roleManager.CreateAsync(new IdentityRole(roleName));
                    roleResult.Wait();
                }
            }

            //Check if the admin user exists and create it if not
            //Add to the Administrator role

            var user = userManager.FindByEmailAsync(email).GetAwaiter().GetResult();

            if (user == null)
            {
                IdentityUser adminUser = new IdentityUser
                {
                    Email = email,
                    UserName = username
                };

                var newUser = userManager.CreateAsync(adminUser, "erO$15Vd6p").GetAwaiter().GetResult();
                if (newUser.Succeeded)
                {
                    userManager.AddToRoleAsync(adminUser, adminRole).GetAwaiter().GetResult();
                }
            }
        }
    }
}