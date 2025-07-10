using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Stripe;
using System.Globalization;
using TravelFinalProject.DAL;
using TravelFinalProject.Interfaces;
using TravelFinalProject.Models;
using TravelFinalProject.Services;
using TravelFinalProject.Services.Implementations;

namespace TravelFinalProject
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddControllersWithViews();
            builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
            builder.Services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new List<CultureInfo> { new CultureInfo("en"), new CultureInfo("az") };
                options.DefaultRequestCulture = new RequestCulture("en");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;

                options.RequestCultureProviders = new List<IRequestCultureProvider>
    {
        new QueryStringRequestCultureProvider { QueryStringKey = "langCode", UIQueryStringKey = "langCode" },
        new CookieRequestCultureProvider()
    };
            });




            builder.Services.AddIdentity<AppUser, IdentityRole>(opt =>
            {
                opt.Password.RequiredLength = 8;
                opt.Password.RequireNonAlphanumeric = false;
                opt.User.RequireUniqueEmail = true;
                opt.Lockout.MaxFailedAccessAttempts = 5;
                opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                opt.SignIn.RequireConfirmedEmail = true;
            }).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();
            builder.Services.AddDbContext<AppDbContext>(opt =>
              opt.UseSqlServer(builder.Configuration.GetConnectionString("Default"))
                );
            builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));
            builder.Services.AddScoped<IEmailService, EmailService>();
            builder.Services.AddScoped<INotificationService, NotificationService>();
            builder.Services.AddHostedService<NotificationBackgroundService>();
            builder.Services.AddHttpClient();
            builder.Services.AddMemoryCache();
            builder.Services.AddScoped<ICurrencyService, CurrencyService>();
            builder.Services.Configure<CurrencySettings>(builder.Configuration.GetSection("CurrencySettings"));
            builder.Services.AddScoped<LayoutService>();


            var app = builder.Build();


            var localizationOptions = app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value;
            app.UseRequestLocalization(localizationOptions);
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseStaticFiles();
            //app.UseMiddleware<GlobalExceptionHandlerMiddleware>();


            StripeConfiguration.ApiKey = builder.Configuration["Stripe:Secretkey"];

            app.MapControllerRoute(
               "Admin",
               "{Area:exists}/{controller=home}/{action=Index}/{Id?}"
               );
            app.MapControllerRoute(
                "default",
                "{controller=home}/{action=Index}/{Id?}"
                );

            app.Run();
        }
    }
}
