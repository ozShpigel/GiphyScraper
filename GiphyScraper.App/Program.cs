using System.Net.Http.Json;

var apiKey = Environment.GetEnvironmentVariable("GIPHY_API_KEY")
    ?? throw new InvalidOperationException("GIPHY_API_KEY is not set");

using var http = new HttpClient();
var url = $"https://api.giphy.com/v1/gifs/trending?api_key={apiKey}";

var response = await http.GetFromJsonAsync<GiphyResponse>(url);

foreach (var gif in response!.Data)
    Console.WriteLine(gif.Url);

record GiphyResponse(List<Gif> Data);
record Gif(string Url);