using System.Net.Http.Json;
using GiphyScraper.Core.Models;

namespace GiphyScraper.Core.Services;

public class GiphyClient(HttpClient http, string apiKey) : IGiphyClient
{
    private const string BaseUrl = "https://api.giphy.com/v1/gifs";
    private const int MaxSearchTermLength = 50;

    public Task<IReadOnlyList<string>> GetTrendingUrlsAsync(CancellationToken cancellationToken = default) =>
        GetUrlsAsync($"{BaseUrl}/trending?api_key={apiKey}", cancellationToken);

    public Task<IReadOnlyList<string>> SearchUrlsAsync(string term, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(term);

        if (term.Length > MaxSearchTermLength)
            throw new ArgumentException($"Search term must be at most {MaxSearchTermLength} characters.", nameof(term));

        return GetUrlsAsync($"{BaseUrl}/search?api_key={apiKey}&q={Uri.EscapeDataString(term)}", cancellationToken);
    }

    private async Task<IReadOnlyList<string>> GetUrlsAsync(string url, CancellationToken cancellationToken)
    {
        var response = await http.GetFromJsonAsync<GiphyResponse>(url, cancellationToken);
        return response?.Data.Select(gif => gif.Url).ToList() ?? [];
    }
}