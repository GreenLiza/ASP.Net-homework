using GoodNewsAggregator.Abstractions;
using GoodNewsAggregator.Abstractions.Repositories;
using GoodNewsAggregator.Abstractions.Services;
using GoodNewsAggregator.Business;
using GoodNewsAggregator.Data;
using GoodNewsAggregator.Data.Entities;
using GoodNewsAggregator.Repositories.Implementations;
using GoodNewsAggregator.Ropositories;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

namespace GoodNewsAggregator.MVC
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            builder.Services.AddDbContext<NewsAggregatorContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });

            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            builder.Services.AddScoped<INewsRepository, NewsRepository>();
            builder.Services.AddScoped<IRepository<Comment>, Repository<Comment>>();
            builder.Services.AddScoped<IRepository<RatingWord>, Repository<RatingWord>>();
            builder.Services.AddScoped<IRepository<Role>, RoleRepository>();
            builder.Services.AddScoped<IRepository<Source>, Repository<Source>>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();

            builder.Services.AddTransient<INewsService, NewsService>();
            builder.Services.AddTransient<IUserService, UserService>();
            builder.Services.AddTransient<IRoleService, RoleService>();
            builder.Services.AddTransient<ISourceService, SourceService>();

            builder.Services.AddAutoMapper(typeof(Program));

            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = new PathString("/User/LogIn");
                    options.AccessDeniedPath = new PathString("/User/LogIn");
                });
            builder.Services.AddAuthorization();

            builder.Services.AddControllersWithViews();

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
                name: "title",
                pattern: "{controller=News}/{action=Details}/{title}");

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=News}/{action=Index}/{id?}");



            app.Run();
        }
    }
}