using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nest;
using SearchEngine.Autocomplete.Application.Interfaces;
using SearchEngine.Autocomplete.Infrastructure.Services;
using System;

namespace SearchEngine.Autocomplete.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var settings = new ConnectionSettings(new Uri(configuration["elasticsearch:url"]));

            var client = new ElasticClient(settings);

            services.AddSingleton<IElasticClient>(client);

            services.AddScoped<IRealEstateEntityService, RealEstateEntityService>();

            return services;
        }
    }
}