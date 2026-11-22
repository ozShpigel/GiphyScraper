namespace GiphyScraper.Core.Models;

public record GiphyResponse(List<Gif> Data);

public record Gif(string Url);