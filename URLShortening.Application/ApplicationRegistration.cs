using Microsoft.Extensions.DependencyInjection;
using URLShortening.Application.Interfaces;
using URLShortening.Application.Services;

namespace URLShortening.Application
{
    public static class ApplicationRegistration
	{
        public static IServiceCollection Add(this IServiceCollection services)
        {
            services.AddScoped<IUrlShorteningService, UrlShorteningService>();
            return services;
        }
    }
}

