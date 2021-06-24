using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using SearchEngine.Autocomplete.Application;
using SearchEngine.Autocomplete.Infrastructure;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;

namespace SearchEngine.Autocomplete.Api
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
            services.AddControllers()
                 .AddJsonOptions(options =>
                 {
                     options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                 });

            services.AddApplication();

            services.AddInfrastructure(Configuration);

            services.AddSwaggerGen(c =>
            {
                c.ResolveConflictingActions(descriptions =>
                {
                    return descriptions.First();
                });

                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Autocomplete Api",
                    Description = "A simple API to demonstrate autocomplete feature over Elasticsearch",
                    Contact = new OpenApiContact
                    {
                        Name = "David Villanueva",
                        Email = "daviva70@gmail.com",
                        Url = new Uri("https://www.linkedin.com/in/dvillanuevav/")
                    }
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Autocomplete API v1");
                    c.RoutePrefix = string.Empty;
                });
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}