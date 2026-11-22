namespace GiphyScraper.Core.Configuration;

public class GiphyOptions
{
    public const string SectionName = "Giphy";

    public string ApiKey { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = "https://api.giphy.com/v1/gifs/";
    public TimeSpan CacheDuration { get; set; } = TimeSpan.FromMinutes(5);
}