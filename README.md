# Giphy Scraper

![CI](https://github.com/ozShpigel/GiphyScraper/actions/workflows/ci.yml/badge.svg)

Fetches GIF URLs from the [Giphy API](https://developers.giphy.com/) — trending GIFs and search by term — with caching and a minimal web UI.

## Features

- Trending GIFs and search by term
- In-memory cache (5 min) to prevent redundant API calls
- Safe concurrent access — parallel requests for the same term trigger a single API call
- Console app and web UI with a GIF gallery

## Project Structure

| Project | Purpose |
|---|---|
| `GiphyScraper.Core` | Giphy client, caching, configuration |
| `GiphyScraper.App` | Interactive console app |
| `GiphyScraper.Web` | Minimal API + static HTML UI |
| `GiphyScraper.Tests` | xUnit tests |

## Setup

Requires [.NET 10 SDK](https://dotnet.microsoft.com/download) and a [Giphy API key](https://developers.giphy.com/dashboard/).

```bash
dotnet user-secrets set "Giphy:ApiKey" "<your-key>" --project GiphyScraper.App
dotnet user-secrets set "Giphy:ApiKey" "<your-key>" --project GiphyScraper.Web
```

Alternatively, set the environment variable `Giphy__ApiKey`.

## Run

```bash
dotnet run --project GiphyScraper.App   # console: type a term, 'trending' or 'exit'
dotnet run --project GiphyScraper.Web   # web UI: open the URL shown in the output
dotnet test
```

## Design Notes

- **Decorator** — `CachedGiphyClient` wraps `GiphyClient` and adds caching without changing it.
- **Per-key locking** — `SemaphoreSlim` per cache key with double-checked locking prevents cache stampede.
- **Shared registration** — `AddGiphyClient()` registers everything once for both apps.
- **Trending "of the day"** — the trending endpoint returns the current feed. The `trending_datetime` field reflects when a GIF first trended, so it is not used for filtering.