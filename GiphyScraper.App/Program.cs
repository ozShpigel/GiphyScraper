using System.Diagnostics;
using GiphyScraper.Core.Services;
using Microsoft.Extensions.Caching.Memory;

var apiKey = Environment.GetEnvironmentVariable("GIPHY_API_KEY");
if (string.IsNullOrWhiteSpace(apiKey))
{
    Console.Error.WriteLine("Error: GIPHY_API_KEY environment variable is not set.");
    return 1;
}

using var http = new HttpClient();
using var cache = new MemoryCache(new MemoryCacheOptions());
IGiphyClient giphy = new CachedGiphyClient(new GiphyClient(http, apiKey), cache, TimeSpan.FromMinutes(5));

Console.WriteLine("Type a search term, 'trending', or 'exit'.");

while (true)
{
    Console.Write("> ");
    var input = Console.ReadLine()?.Trim();

    if (input is null || input.Equals("exit", StringComparison.OrdinalIgnoreCase))
        break;

    if (input.Length == 0)
        continue;

    try
    {
        var stopwatch = Stopwatch.StartNew();

        var urls = input.Equals("trending", StringComparison.OrdinalIgnoreCase)
            ? await giphy.GetTrendingUrlsAsync()
            : await giphy.SearchUrlsAsync(input);

        if (urls.Count == 0)
            Console.WriteLine("No GIFs found.");

        foreach (var url in urls)
            Console.WriteLine(url);

        Console.WriteLine($"{urls.Count} results in {stopwatch.ElapsedMilliseconds} ms");
    }
    catch (ArgumentException ex)
    {
        Console.Error.WriteLine($"Invalid input: {ex.Message}");
    }
    catch (HttpRequestException ex)
    {
        Console.Error.WriteLine($"Giphy request failed: {ex.Message}");
    }
}

return 0;