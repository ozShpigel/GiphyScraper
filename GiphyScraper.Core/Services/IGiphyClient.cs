using GiphyScraper.Core.Models;

namespace GiphyScraper.Core.Services;

public interface IGiphyClient
{
    Task<IReadOnlyList<GifResult>> GetTrendingUrlsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<GifResult>> SearchUrlsAsync(string term, CancellationToken cancellationToken = default);
}