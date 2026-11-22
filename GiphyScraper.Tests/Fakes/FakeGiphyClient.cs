using GiphyScraper.Core.Services;

namespace GiphyScraper.Tests.Fakes;

public class FakeGiphyClient : IGiphyClient
{
    public int CallCount { get; private set; }

    public Task<IReadOnlyList<string>> GetTrendingUrlsAsync(CancellationToken cancellationToken = default)
    {
        CallCount++;
        return Task.FromResult<IReadOnlyList<string>>(["trending"]);
    }

    public Task<IReadOnlyList<string>> SearchUrlsAsync(string term, CancellationToken cancellationToken = default)
    {
        CallCount++;
        return Task.FromResult<IReadOnlyList<string>>([term]);
    }
}