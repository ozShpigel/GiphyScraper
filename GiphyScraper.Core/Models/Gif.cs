using System.Text.Json.Serialization;

namespace GiphyScraper.Core.Models;

public record GiphyResponse(List<Gif> Data);

public record Gif(string Url, GifImages? Images);

public record GifImages([property: JsonPropertyName("fixed_width")] GifImage? FixedWidth);

public record GifImage(string Url);