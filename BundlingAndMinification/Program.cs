using BundlingAndMinification.Data;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Options;
using System.IO.Compression;
using Microsoft.EntityFrameworkCore;

namespace BundlingAndMinification
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddResponseCaching();

            builder.Services.Configure<GzipCompressionProviderOptions>(options =>
            {
                options.Level = CompressionLevel.NoCompression;
            });

            builder.Services.AddDbContext<DbEmployeeContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DataConnection")));
            builder.Services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
                options.Providers.Add<GzipCompressionProvider>();
            });

            var redisConfiguration = builder.Configuration.GetSection("Redis:Configuration").Value;

            
            builder.Services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConfiguration;
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

            app.UseAuthorization();

            app.UseResponseCaching();

            app.UseResponseCompression();
            

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
