using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;

namespace IndicadorChileAPI.App.Start
{
    public class Startup
    {
        #region Constructor method
        public Startup()
            : base()
        {

        }
        #endregion



        #region Destroyer method
        ~Startup()
        {

        }
        #endregion



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
            Builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });
            Builder.Services.AddEndpointsApiExplorer();
            Builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Indicadores de Chile",
                    Version = "v1"
                });
            });
        }

        private static void Configure(WebApplication App)
        {
            // Configure the HTTP request pipeline.
            if (App.Environment.IsDevelopment())
            {
                App.UseSwagger();
                App.UseSwaggerUI();
            }

            App.UseHttpsRedirection();

            App.UseAuthorization();

            App.MapAreaControllerRoute(
                name: "MyAreaSII",
                areaName: "SII",
                pattern: "SII/{controller=Home}/{action=Index}/{id?}"
            );

            App.MapControllers();
        }
        #endregion
    }
}
