using HotelListing.Core.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace HotelListing.Core
{
    public static class AppExtensions
    {
        public static void ConfigureExceptionHandler(this IApplicationBuilder app)
        {
            app.UseExceptionHandler(error =>
            {
                error.Run(async context =>
                {
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    context.Response.ContentType = "application/json";
                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if (contextFeature != null)
                    {
                        Log.Error($"Error in {contextFeature.Error} method.");

                        await context.Response.WriteAsync(new Error
                        {
                            StatusCode = context.Response.StatusCode,
                            Message = "Internal server error. Please try again."
                        }.ToString());
                    }
                });
            });
        }

        public static void ConfigureSwagger(this IApplicationBuilder app, IConfiguration Configuration)
        {
            var swaggerOptions = new SwaggerOptions();
            Configuration.GetSection("Swagger").Bind(swaggerOptions);
            app.UseSwagger(option => option.RouteTemplate = swaggerOptions.JsonRoute);
            app.UseSwaggerUI(option =>
            {
                foreach (var currentVersion in swaggerOptions.Versions)
                {
                    //option.SwaggerEndpoint(currentVersion.UiEndpoint, $"{swaggerOptions.Title} {currentVersion.Name}");
                    string swaggerJsonBasePath = string.IsNullOrWhiteSpace(option.RoutePrefix) ? "." : "..";
                    option.SwaggerEndpoint($"{swaggerJsonBasePath}{currentVersion.UiEndpoint}", $"{swaggerOptions.Title} {currentVersion.Name}");
                }
            });
        }
    }
}
