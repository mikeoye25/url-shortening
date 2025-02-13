using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using URLShortening.Application.Contracts;
using URLShortening.Infrastructure.Repositories;

namespace URLShortening.Infrastructure
{
    public static class InfrastructureRegistration
	{
        public static IServiceCollection Add(this IServiceCollection services, IConfiguration configuration)
        {
            // uses PostgreSQL - a persistent data store
            services.AddEntityFrameworkNpgsql().AddDbContext<URLShorteningContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("URLShorteningDbConnection")));
            services.AddScoped(typeof(IAsyncRepository<>), typeof(RepositoryBase<>));
            services.AddScoped<IShortenedUrlRepository, ShortenedUrlRepository>();
            return services;
        }
    }
}

