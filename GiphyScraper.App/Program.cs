using System.Diagnostics;
using GiphyScraper.Core;
using GiphyScraper.Core.Configuration;
using GiphyScraper.Core.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = Host.CreateApplicationBuilder(args);
builder.Configuration.AddUserSecrets<Program>();
builder.Logging.ClearProviders();

if (string.IsNullOrWhiteSpace(builder.Configuration[$"{GiphyOptions.SectionName}:ApiKey"]))
{
    Console.Error.WriteLine("Error: Giphy:ApiKey is not configured.");
    Console.Error.WriteLine("Run: dotnet user-secrets set \"Giphy:ApiKey\" \"<your-key>\" --project GiphyScraper.App");
    return 1;
}

builder.Services.AddGiphyClient(builder.Configuration);

using var host = builder.Build();
var giphy = host.Services.GetRequiredService<IGiphyClient>();
_ = giphy.GetTrendingUrlsAsync(); // Warm up the cache in the background

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

        var gifs = input.Equals("trending", StringComparison.OrdinalIgnoreCase)
            ? await giphy.GetTrendingUrlsAsync()
            : await giphy.SearchUrlsAsync(input);

        if (gifs.Count == 0)
            Console.WriteLine("No GIFs found.");

        foreach (var gif in gifs)
            Console.WriteLine(gif.Url);

        Console.WriteLine($"{gifs.Count} results in {stopwatch.ElapsedMilliseconds} ms");
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