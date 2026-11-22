using GiphyScraper.Core.Configuration;
using GiphyScraper.Core.Services;
using GiphyScraper.Tests.Fakes;
using Microsoft.Extensions.Options;

namespace GiphyScraper.Tests;

public class GiphyClientTests
{
    private const string ResponseJson =
        """{"data":[{"url":"https://giphy.com/gifs/a","images":{"fixed_width":{"url":"https://media.giphy.com/a.gif"}}}]}""";

    private readonly FakeHttpMessageHandler _handler = new(ResponseJson);
    private readonly GiphyClient _client;

    public GiphyClientTests()
    {
        var http = new HttpClient(_handler) { BaseAddress = new Uri("https://api.giphy.com/v1/gifs/") };
        var options = Options.Create(new GiphyOptions { ApiKey = "test-key" });
        _client = new GiphyClient(http, options);
    }

    [Fact]
    public async Task GetTrendingUrlsAsync_ReturnsGifsFromResponse()
    {
        var gifs = await _client.GetTrendingUrlsAsync();

        var gif = Assert.Single(gifs);
        Assert.Equal("https://giphy.com/gifs/a", gif.Url);
        Assert.Equal("https://media.giphy.com/a.gif", gif.PreviewUrl);
    }

    [Fact]
    public async Task SearchUrlsAsync_EncodesSearchTerm()
    {
        await _client.SearchUrlsAsync("funny cats");

        Assert.Contains("q=funny%20cats", _handler.LastRequestUri!.AbsoluteUri);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task SearchUrlsAsync_EmptyTerm_Throws(string term)
    {
        await Assert.ThrowsAsync<ArgumentException>(() => _client.SearchUrlsAsync(term));
    }

    [Fact]
    public async Task SearchUrlsAsync_TermLongerThan50Characters_Throws()
    {
        await Assert.ThrowsAsync<ArgumentException>(() => _client.SearchUrlsAsync(new string('a', 51)));
    }
}