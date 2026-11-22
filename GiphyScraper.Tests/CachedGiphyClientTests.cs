using GiphyScraper.Core.Services;
using GiphyScraper.Tests.Fakes;
using Microsoft.Extensions.Caching.Memory;

namespace GiphyScraper.Tests;

public class CachedGiphyClientTests
{
    private readonly FakeGiphyClient _inner = new();
    private readonly CachedGiphyClient _client;

    public CachedGiphyClientTests()
    {
        var cache = new MemoryCache(new MemoryCacheOptions());
        _client = new CachedGiphyClient(_inner, cache, TimeSpan.FromMinutes(5));
    }

    [Fact]
    public async Task GetTrendingUrlsAsync_CalledTwice_CallsApiOnce()
    {
        await _client.GetTrendingUrlsAsync();
        await _client.GetTrendingUrlsAsync();

        Assert.Equal(1, _inner.CallCount);
    }

    [Fact]
    public async Task SearchUrlsAsync_SameTermTwice_CallsApiOnce()
    {
        await _client.SearchUrlsAsync("cats");
        await _client.SearchUrlsAsync("cats");

        Assert.Equal(1, _inner.CallCount);
    }

    [Fact]
    public async Task SearchUrlsAsync_IgnoresCaseAndWhitespace()
    {
        await _client.SearchUrlsAsync("cats");
        await _client.SearchUrlsAsync("  CATS ");

        Assert.Equal(1, _inner.CallCount);
    }

    [Fact]
    public async Task SearchUrlsAsync_DifferentTerms_CallsApiForEach()
    {
        await _client.SearchUrlsAsync("cats");
        await _client.SearchUrlsAsync("dogs");

        Assert.Equal(2, _inner.CallCount);
    }

    [Fact]
    public async Task SearchUrlsAsync_ConcurrentRequestsForSameTerm_CallsApiOnce()
    {
        var requests = Enumerable.Range(0, 10).Select(_ => _client.SearchUrlsAsync("cats"));

        await Task.WhenAll(requests);

        Assert.Equal(1, _inner.CallCount);
    }    
}