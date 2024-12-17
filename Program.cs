using Serilog;
using ShopifyTool.Models;
using ShopifyTool.Services;

namespace ShopifyTool
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //Configure Serilog.
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration) // Read settings from appsettings.json
                .Enrich.FromLogContext() // Add context to logs
                .WriteTo.Console() // Logs to console
                .WriteTo.File("Logs/app-log-.txt", rollingInterval: RollingInterval.Day) // Logs to file
                .CreateLogger();

            // Replace default logger
            builder.Logging.ClearProviders();
            builder.Logging.AddSerilog();

            //Register the configuration.
            builder.Services.Configure<ShopifySettings>(
                builder.Configuration.GetSection("Shopify"));

            //Register the Shopify Service with a HttpClient and the TokenService.
            builder.Services.AddHttpClient<ShopifyService>();
            builder.Services.AddSingleton<TokenService>();

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
                app.UseHttpsRedirection();
            }

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Shopify}/{action=Index}");

            app.Run();
        }
    }
}
