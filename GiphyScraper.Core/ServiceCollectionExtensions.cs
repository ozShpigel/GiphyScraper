using GiphyScraper.Core.Configuration;
using GiphyScraper.Core.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace GiphyScraper.Core;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGiphyClient(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<GiphyOptions>(configuration.GetSection(GiphyOptions.SectionName));
        services.AddMemoryCache();

        services.AddHttpClient<GiphyClient>((provider, client) =>
                client.BaseAddress = new Uri(provider.GetRequiredService<IOptions<GiphyOptions>>().Value.BaseUrl))
            .ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler { PooledConnectionLifetime = TimeSpan.FromMinutes(2) })
            .SetHandlerLifetime(Timeout.InfiniteTimeSpan);

        services.AddSingleton<IGiphyClient>(provider => new CachedGiphyClient(
            provider.GetRequiredService<GiphyClient>(),
            provider.GetRequiredService<IMemoryCache>(),
            provider.GetRequiredService<IOptions<GiphyOptions>>().Value.CacheDuration));

        return services;
    }
}