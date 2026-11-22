using Microsoft.Extensions.Caching.Memory;

namespace GiphyScraper.Core.Services;

public class CachedGiphyClient(IGiphyClient inner, IMemoryCache cache, TimeSpan cacheDuration) : IGiphyClient
{
    public Task<IReadOnlyList<string>> GetTrendingUrlsAsync(CancellationToken cancellationToken = default) =>
        GetOrFetchAsync("trending", () => inner.GetTrendingUrlsAsync(cancellationToken));

    public Task<IReadOnlyList<string>> SearchUrlsAsync(string term, CancellationToken cancellationToken = default) =>
        GetOrFetchAsync($"search:{term?.Trim().ToLowerInvariant()}", () => inner.SearchUrlsAsync(term!, cancellationToken));

    private async Task<IReadOnlyList<string>> GetOrFetchAsync(string key, Func<Task<IReadOnlyList<string>>> fetch)
    {
        if (cache.TryGetValue(key, out IReadOnlyList<string>? cached) && cached is not null)
            return cached;

        var result = await fetch();
        cache.Set(key, result, cacheDuration);
        return result;
    }
}