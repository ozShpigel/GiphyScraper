using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Caching.Memory;

namespace GiphyScraper.Core.Services;

public class CachedGiphyClient(IGiphyClient inner, IMemoryCache cache, TimeSpan cacheDuration) : IGiphyClient
{
    private readonly ConcurrentDictionary<string, SemaphoreSlim> _locks = new();

    public Task<IReadOnlyList<string>> GetTrendingUrlsAsync(CancellationToken cancellationToken = default) =>
        GetOrFetchAsync("trending", () => inner.GetTrendingUrlsAsync(cancellationToken), cancellationToken);

    public Task<IReadOnlyList<string>> SearchUrlsAsync(string term, CancellationToken cancellationToken = default) =>
        GetOrFetchAsync(
            $"search:{term?.Trim().ToLowerInvariant()}",
            () => inner.SearchUrlsAsync(term!, cancellationToken),
            cancellationToken);

    private async Task<IReadOnlyList<string>> GetOrFetchAsync(
        string key, Func<Task<IReadOnlyList<string>>> fetch, CancellationToken cancellationToken)
    {
        if (TryGetCached(key, out var cached))
            return cached;

        var keyLock = _locks.GetOrAdd(key, _ => new SemaphoreSlim(1, 1));
        await keyLock.WaitAsync(cancellationToken);
        try
        {
            if (TryGetCached(key, out cached))
                return cached;

            var result = await fetch();
            cache.Set(key, result, cacheDuration);
            return result;
        }
        finally
        {
            keyLock.Release();
        }
    }

    private bool TryGetCached(string key, [NotNullWhen(true)] out IReadOnlyList<string>? value) =>
        cache.TryGetValue(key, out value) && value is not null;
}