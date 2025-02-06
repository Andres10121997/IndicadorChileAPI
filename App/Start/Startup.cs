using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
            Builder.Services.AddControllers();
            Builder.Services.AddEndpointsApiExplorer();
            Builder.Services.AddSwaggerGen();
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

            App.MapControllers();
        }
        #endregion
    }
}
