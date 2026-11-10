using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Scalar.AspNetCore;
using System;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;

namespace IndicadorChileAPI.App.Start
{
    public class Startup
    {
        #region Init
        public static WebApplication InitApp(string[] args)
        {
            #region Objects
            WebApplicationBuilder builder;
            WebApplication app;
            #endregion

            builder = WebApplication.CreateBuilder(args: args);

            ConfigureServices(Builder: builder);

            app = builder.Build();

            Configure(App: app);

            return app;
        }
        #endregion



        #region Configure
        private static void ConfigureServices(WebApplicationBuilder Builder)
        {
            Builder.Services.AddMvc()
                .AddJsonOptions(
                    configure: options =>
                    {
                        options.JsonSerializerOptions.Converters.Add(
                            item: new JsonStringEnumConverter()
                        );
                    }
                );

            Builder.Services.AddEndpointsApiExplorer();

            // Limitar peticiones por IP.
            // https://www.youtube.com/shorts/EoJl5wgE5UQ
            Builder.Services.AddRateLimiter(
                options =>
                {
                    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

                    options.AddFixedWindowLimiter(
                        policyName: "Limiter",
                        fixedWindowOptions =>
                        {
                            fixedWindowOptions.PermitLimit = 4; // Solicitudes máximas permitidas
                            fixedWindowOptions.Window = TimeSpan.FromSeconds(seconds: 15); // Ventana de tiempo
                            fixedWindowOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst; // Cómo manejar solicitudes en cola
                            fixedWindowOptions.QueueLimit = 1; // Máximo de solicitudes para hacer cola
                        }
                    );
                }
            );

            Builder.Services.AddOpenApi();
        }

        private static void Configure(WebApplication App)
        {
            // Configure the HTTP request pipeline.
            if (App.Environment.IsDevelopment())
            {
                App.MapOpenApi();
                App.MapScalarApiReference();
            }

            App.UseRateLimiter();

            // Forces redirection to HTTPS
            App.UseHttpsRedirection();

            App.UseAuthorization();

            App.MapControllers()
               .RequireRateLimiting(
                   policyName: "Limiter"
               );
        }
        #endregion
    }
}