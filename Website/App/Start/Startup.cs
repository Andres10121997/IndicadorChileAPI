using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;
using Website.App.Support;

namespace Website.App.Start
{
    public class Startup
    {
        public static WebApplication InitApp(string[] args)
        {
            WebApplicationBuilder builder;
            WebApplication app;

            builder = WebApplication.CreateBuilder(args);

            ConfigureServices(builder);

            app = builder.Build();

            Configure(app);

            return app;
        }



        #region Configure
        private static void ConfigureServices(WebApplicationBuilder Builder)
        {
            IdentityModelEventSource.ShowPII = true;
            //To use MVC we have to explicitly declare we are using it. Doing so will prevent a System.InvalidOperationException.
            Builder.Services.AddControllersWithViews();
            Builder.Services.AddRazorPages();
            Builder.Services.AddServerSideBlazor();
            Builder.Services.AddControllersWithViews();
            // https://learn.microsoft.com/es-mx/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-7.0
            // Configure the HTTP request pipeline.
            Builder.Services.ConfigureSameSiteNoneCookies();
        }

        private static void Configure(WebApplication App)
        {
            // Configure the HTTP request pipeline.
            if (!App.Environment.IsDevelopment())
            {
                App.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                App.UseHsts();
            }

            App.UseStaticFiles();
            App.UseCookiePolicy();

            App.UseRouting();
            App.UseAuthentication();
            App.UseAuthorization();

            // Map controller route.
            App.MapAreaControllerRoute(
                name: "AccountRoute",
                areaName: "Account",
                pattern: "Account/{controller=Home}/{action=Index}/{id?}"
            );

            App.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}"
            );

            App.MapBlazorHub();
        }
        #endregion
    }
}
