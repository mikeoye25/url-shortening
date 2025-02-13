using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using URLShortening.Application.AppSettings;
using URLShortening.Application.Interfaces;
using URLShortening.Application.Services;

namespace URLShortening.Application
{
    public static class ApplicationRegistration
	{
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            var fixedOptions = new FixedOptions();
            configuration.GetSection("FixedOptions").Bind(fixedOptions);
            services.AddSingleton(fixedOptions);

            services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = 429;
                options.AddPolicy(policyName: "fixed", httpContext =>
                {
                    var clientIP = httpContext.Connection.RemoteIpAddress!.ToString();
                    return RateLimitPartition.GetFixedWindowLimiter(
                        clientIP, _ => new FixedWindowRateLimiterOptions
                        {
                            AutoReplenishment = true,
                            PermitLimit = fixedOptions!.PermitLimit,
                            Window = TimeSpan.FromMinutes(fixedOptions.Window),
                            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                            QueueLimit = fixedOptions.QueueLimit,
                        });
                });
            });

            services.AddScoped<IUrlShorteningService, UrlShorteningService>();
            return services;
        }
    }
}

