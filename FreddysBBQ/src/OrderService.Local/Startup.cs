using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using OrderService.Providers;
using Steeltoe.Connector.MySql.EFCore;
using System;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Steeltoe.Security.Authentication.CloudFoundry;
using Microsoft.AspNetCore.HttpOverrides;
using System.Text;

namespace OrderService.Local
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddSwaggerGen(config =>
            {
                config.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Order Service",
                    Description = "Order Service Swagger Doc",

                    License = new OpenApiLicense
                    {
                        Name = "Apache License 2.0",
                        Url = new Uri("https://raw.githubusercontent.com/SteeltoeOSS/Samples/master/License.txt")
                    }

                });

                var jwtSecurityScheme = new OpenApiSecurityScheme
                {
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Name = "JWT Authentication",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Description = "Add your JWT Bearer token below (do not prefix with 'Bearer')",

                    Reference = new OpenApiReference
                    {
                        Id = JwtBearerDefaults.AuthenticationScheme,
                        Type = ReferenceType.SecurityScheme
                    }
                };

                config.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);

                config.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    { jwtSecurityScheme, Array.Empty<string>() }
                });
            });

           services.AddAuthentication(x =>
                {
                    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddCloudFoundryJwtBearer(Configuration, (options, configurer) => {
                    var uaa = "http://localhost:8080/uaa";

                    var defaultOptions = new CloudFoundryJwtBearerOptions();

                    options.RequireHttpsMetadata = false;
                    options.ClaimsIssuer = uaa + CloudFoundryDefaults.AccessTokenUri;
                    options.BackchannelHttpHandler = CloudFoundryHelper.GetBackChannelHandler(false);
                    options.TokenValidationParameters = CloudFoundryHelper.GetTokenValidationParameters(
                        defaultOptions.TokenValidationParameters, 
                        uaa + CloudFoundryDefaults.JwtTokenUri, 
                        options.BackchannelHttpHandler, false,
                        new AuthServerOptions { AdditionalAudiences = new string[] { "orders" } });
                });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("Orders", policy => policy.RequireClaim("scope", "orders.read"));
                options.AddPolicy("AdminOrders", policy => policy.RequireClaim("scope", "orders.admin"));
            }); 

            services.AddDbContext<IOrderStorage, OrderStorage>(options => options.UseMySql(Configuration));
            services.AddTransient<IOrderStorage, OrderStorage>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "OrderService Demo"));
            }

            app.UseRouting();

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedProto
            });

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
