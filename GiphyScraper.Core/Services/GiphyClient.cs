using System.Net.Http.Json;
using GiphyScraper.Core.Configuration;
using GiphyScraper.Core.Models;
using Microsoft.Extensions.Options;

namespace GiphyScraper.Core.Services;

public class GiphyClient(HttpClient http, IOptions<GiphyOptions> options) : IGiphyClient
{
    private const int MaxSearchTermLength = 50;
    private readonly string _apiKey = options.Value.ApiKey;

    public Task<IReadOnlyList<string>> GetTrendingUrlsAsync(CancellationToken cancellationToken = default) =>
        GetUrlsAsync($"trending?api_key={_apiKey}", cancellationToken);

    public Task<IReadOnlyList<string>> SearchUrlsAsync(string term, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(term);

        if (term.Length > MaxSearchTermLength)
            throw new ArgumentException($"Search term must be at most {MaxSearchTermLength} characters.", nameof(term));

        return GetUrlsAsync($"search?api_key={_apiKey}&q={Uri.EscapeDataString(term)}", cancellationToken);
    }

    private async Task<IReadOnlyList<string>> GetUrlsAsync(string relativeUrl, CancellationToken cancellationToken)
    {
        var response = await http.GetFromJsonAsync<GiphyResponse>(relativeUrl, cancellationToken);
        return response?.Data.Select(gif => gif.Url).ToList() ?? [];
    }
}