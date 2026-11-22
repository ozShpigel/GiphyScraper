using System.Net.Http.Json;
using GiphyScraper.Core.Services;

var apiKey = Environment.GetEnvironmentVariable("GIPHY_API_KEY");
if (string.IsNullOrWhiteSpace(apiKey))
{
    Console.Error.WriteLine("Error: GIPHY_API_KEY environment variable is not set.");
    return 1;
}

using var http = new HttpClient();
IGiphyClient giphy = new GiphyClient(http, apiKey);

try
{
    var urls = args.Length == 0
        ? await giphy.GetTrendingUrlsAsync()
        : await giphy.SearchUrlsAsync(string.Join(" ", args));

    if (urls.Count == 0)
        Console.WriteLine("No GIFs found.");

    foreach (var url in urls)
        Console.WriteLine(url);

    return 0;
}
catch (ArgumentException ex)
{
    Console.Error.WriteLine($"Invalid input: {ex.Message}");
    return 1;
}
catch (HttpRequestException ex)
{
    Console.Error.WriteLine($"Giphy request failed: {ex.Message}");
    return 1;
}