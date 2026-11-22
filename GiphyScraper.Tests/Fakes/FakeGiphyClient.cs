using GiphyScraper.Core.Models;
using GiphyScraper.Core.Services;

namespace GiphyScraper.Tests.Fakes;

public class FakeGiphyClient : IGiphyClient
{
    private int _callCount;
    public int CallCount => _callCount;

    public async Task<IReadOnlyList<GifResult>> GetTrendingUrlsAsync(CancellationToken cancellationToken = default)
    {
        Interlocked.Increment(ref _callCount);
        await Task.Delay(50, cancellationToken);
        return [new GifResult("trending", string.Empty)];
    }

    public async Task<IReadOnlyList<GifResult>> SearchUrlsAsync(string term, CancellationToken cancellationToken = default)
    {
        Interlocked.Increment(ref _callCount);
        await Task.Delay(50, cancellationToken);
        return [new GifResult(term, string.Empty)];
    }
}