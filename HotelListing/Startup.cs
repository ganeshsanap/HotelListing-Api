using AspNetCoreRateLimit;
using HotelListing.Core;
using HotelListing.Core.IRepository;
using HotelListing.Core.Repository;
using HotelListing.Core.Services;
using HotelListing.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace HotelListing
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<DatabaseContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("SqlConnection"))
                );

            // for Caching
            services.AddMemoryCache();

            // for Rate Limiting and Throttling
            services.ConfigureRateLimiting();
            services.AddHttpContextAccessor();

            //services.AddResponseCaching();
            services.ConfigureHttpCacheHeader();

            services.AddAuthentication();
            services.ConfigureIdentity();
            services.ConfigureJWT(Configuration);

            services.AddCors(c =>
            {
                c.AddPolicy("CorsPolicy-AllowAll", builder =>
                builder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());
            });

            services.ConfigureAutoMapper();

            services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();
            services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IAuthManager, AuthManager>();

            //services.AddSwaggerGen(c =>
            //{
            //    c.SwaggerDoc("v1", new OpenApiInfo { Title = "HotelListing", Version = "v1" });
            //});
            services.ConfigureSwaggerGen(Configuration);

            services.AddControllers().AddNewtonsoftJson(o =>
                o.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            );

            // for API versioning
            services.ConfigureVersioning();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.UseSwagger();
            //app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "HotelListing v1"));
            app.ConfigureSwagger(Configuration);

            app.ConfigureExceptionHandler();

            app.UseHttpsRedirection();

            app.UseCors("CorsPolicy-AllowAll");

            // for Caching
            app.UseResponseCaching();

            // for Rate Limiting and Throttling
            app.UseHttpCacheHeaders();
            app.UseIpRateLimiting();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
