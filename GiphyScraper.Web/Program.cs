using GiphyScraper.Core;
using GiphyScraper.Core.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddGiphyClient(builder.Configuration);

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapGet("/api/gifs/trending", async (IGiphyClient giphy, CancellationToken cancellationToken) =>
    Results.Ok(await giphy.GetTrendingUrlsAsync(cancellationToken)));

app.MapGet("/api/gifs/search", async (string q, IGiphyClient giphy, CancellationToken cancellationToken) =>
{
    try
    {
        return Results.Ok(await giphy.SearchUrlsAsync(q, cancellationToken));
    }
    catch (ArgumentException ex)
    {
        return Results.Text(ex.Message, statusCode: StatusCodes.Status400BadRequest);
    }
});

app.Run();