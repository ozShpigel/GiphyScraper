namespace GiphyScraper.Core.Services;

public interface IGiphyClient
{
    Task<IReadOnlyList<string>> GetTrendingUrlsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<string>> SearchUrlsAsync(string term, CancellationToken cancellationToken = default);
}