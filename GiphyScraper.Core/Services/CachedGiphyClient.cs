using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using GiphyScraper.Core.Models;
using Microsoft.Extensions.Caching.Memory;

namespace GiphyScraper.Core.Services;

public class CachedGiphyClient(IGiphyClient inner, IMemoryCache cache, TimeSpan cacheDuration) : IGiphyClient
{
    private readonly ConcurrentDictionary<string, SemaphoreSlim> _locks = new();

    public Task<IReadOnlyList<GifResult>> GetTrendingUrlsAsync(CancellationToken cancellationToken = default) =>
        GetOrFetchAsync("trending", () => inner.GetTrendingUrlsAsync(cancellationToken), cancellationToken);

    public Task<IReadOnlyList<GifResult>> SearchUrlsAsync(string term, CancellationToken cancellationToken = default) =>
        GetOrFetchAsync(
            $"search:{term?.Trim().ToLowerInvariant()}",
            () => inner.SearchUrlsAsync(term!, cancellationToken),
            cancellationToken);

    private async Task<IReadOnlyList<GifResult>> GetOrFetchAsync(
        string key, Func<Task<IReadOnlyList<GifResult>>> fetch, CancellationToken cancellationToken)
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

    private bool TryGetCached(string key, [NotNullWhen(true)] out IReadOnlyList<GifResult>? value) =>
        cache.TryGetValue(key, out value) && value is not null;
}