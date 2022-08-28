using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceAPI.Configure;
using VoiceAPI.DbContextVoiceAPI;
using VoiceAPI.DI;
using VoiceAPI.Extensions;
using VoiceAPI.Middleware;

namespace VoiceAPI
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
            services.ConfigServiceDI();

            // Add dbcontext
            string connectionString = Configuration.GetConnectionString("local");

            string env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (env != null && env.Equals("Production"))
            {
                connectionString = Configuration.GetConnectionString("production");
            }

            services.AddDbContext<VoiceAPIDbContext>(option => option.UseNpgsql(connectionString));

            // Compression
            services.AddResponseCompression();

            // AutoMapper
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            services.AddControllers();

            // Versioning
            services.AddSwaggerGenNewtonsoftSupport();
            services.ConfigureSwagger();

            // Cors
            services.AddCors(opt => opt.AddDefaultPolicy(builder => builder.AllowAnyOrigin()
                                                                           .AllowAnyMethod()
                                                                           .AllowAnyHeader()));

            // JWT
            var tokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                ValidateIssuer = false,
                ValidateAudience = false,
                RequireExpirationTime = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
            };

            services.AddSingleton(tokenValidationParameters);

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(option =>
            {
                option.SaveToken = true;
                option.RequireHttpsMetadata = false;
                option.TokenValidationParameters = tokenValidationParameters;
            });

            // Config
            services.Configure<JwtConfig>(Configuration.GetSection("Jwt"));


            // Disable ModelStateInvalidFilter
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
        {
            app.UseInternalExceptionMiddleware();

            app.UseSwagger();
                
            app.UseSerilogRequestLogging();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseResponseCompression();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            // Versioning
            app.ConfigureSwagger(provider);
        }
    }
}
