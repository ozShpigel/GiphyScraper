using System.Net.Http.Json;
using GiphyScraper.Core.Services;

var apiKey = Environment.GetEnvironmentVariable("GIPHY_API_KEY")
    ?? throw new InvalidOperationException("GIPHY_API_KEY is not set");

using var http = new HttpClient();
IGiphyClient giphy = new GiphyClient(http, apiKey);

var urls = args.Length == 0
    ? await giphy.GetTrendingUrlsAsync()
    : await giphy.SearchUrlsAsync(string.Join(" ", args));

foreach (var url in urls)
    Console.WriteLine(url);