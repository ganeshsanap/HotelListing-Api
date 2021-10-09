using HotelListing.Data;
using HotelListing.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HotelListing
{
    public static class ServiceExtensions
    {
        public static void ConfigureIdentity(this IServiceCollection services)
        {
            var builder = services.AddIdentityCore<ApiUser>(q =>
            {
                q.User.RequireUniqueEmail = true;
            });

            builder = new IdentityBuilder(builder.UserType, typeof(IdentityRole), services);
            builder.AddEntityFrameworkStores<DatabaseContext>().AddDefaultTokenProviders();
        }

        public static void ConfigureJWT(this IServiceCollection services, IConfiguration Configuration)
        {
            var jwtSettings = Configuration.GetSection("Jwt");

            // Open command prompt in Administrator mode
            // in C://windows/system32 run below command
            // setx KEY "your secret key"
            // get KEY is variable name and "your secret key" is value assigned to it 
            // for this api setx KEY "9245fe4a-d402-451c-b9ed-9c1a04247482"
            var key = Environment.GetEnvironmentVariable("KEY");

            services.AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(o =>
                {
                    o.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtSettings.GetSection("Issuer").Value,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
                    };
                });
        }
        
        public static void ConfigureVersioning(this IServiceCollection services)
        {
            services.AddApiVersioning(o =>
            {
                o.ReportApiVersions = true;
                o.AssumeDefaultVersionWhenUnspecified = true;
                o.DefaultApiVersion = new ApiVersion(1, 0);
                o.ApiVersionReader = new HeaderApiVersionReader("api-version");
            });
        }

        public static void ConfigureSwaggerGen(this IServiceCollection services, IConfiguration Configuration)
        {
            services.AddSwaggerGen(swaggerGenOptions =>
            {
                var swaggerOptions = new SwaggerOptions();
                Configuration.GetSection("Swagger").Bind(swaggerOptions);

                foreach (var currentVersion in swaggerOptions.Versions)
                {
                    swaggerGenOptions.SwaggerDoc(currentVersion.Name, new OpenApiInfo
                    {
                        Title = swaggerOptions.Title,
                        Version = currentVersion.Name,
                        Description = swaggerOptions.Description
                    });
                }

                swaggerGenOptions.DocInclusionPredicate((version, desc) =>
                {
                    if (!desc.TryGetMethodInfo(out MethodInfo methodInfo))
                    {
                        return false;
                    }
                    var versions = methodInfo.DeclaringType.GetConstructors()
                        .SelectMany(constructorInfo => constructorInfo.DeclaringType.CustomAttributes
                            .Where(attributeData => attributeData.AttributeType == typeof(ApiVersionAttribute))
                            .SelectMany(attributeData => attributeData.ConstructorArguments
                                .Select(attributeTypedArgument => attributeTypedArgument.Value)));

                    return versions.Any(v => $"{v}" == version);
                });
            });

        }
    }
}
