# Yanga.Module.AlphaVantageApi

`Yanga.Module.AlphaVantageApi` is a small .NET wrapper around the Alpha Vantage API for retrieving daily Forex historical data and mapping it into simple candle objects.

The package currently targets **.NET 10**.

## Features

- Retrieve daily Forex historical prices from Alpha Vantage.
- Convert Alpha Vantage JSON responses into `ForexData` candles.
- Support Alpha Vantage `compact` and `full` output sizes.
- Injectable `HttpClient` support for testing or custom HTTP configuration.

## Installation

Install from NuGet:

```powershell
Install-Package Yanga.Module.AlphaVantageApi
```

Or with the .NET CLI:

```powershell
dotnet add package Yanga.Module.AlphaVantageApi
```

## Requirements

- .NET 10 SDK/runtime
- An Alpha Vantage API key

You can request an API key from Alpha Vantage:

https://www.alphavantage.co/support/#api-key

## Quick Start

```csharp
using Yanga.Module.AlphaVantageApi;

var alphaVantage = new AlphaVantage("YOUR_ALPHA_VANTAGE_API_KEY");

var candles = await alphaVantage.GetForexHistoricalAsync(
    fromSymbol: "EUR",
    toSymbol: "USD",
    outputSize: OutputSize.Compact);

foreach (var candle in candles)
{
    Console.WriteLine(
        $"Date: {candle.Date:yyyy-MM-dd}, " +
        $"Open: {candle.Open}, " +
        $"High: {candle.High}, " +
        $"Low: {candle.Low}, " +
        $"Close: {candle.Close}, " +
        $"Volume: {candle.Volume}");
}
```

## Output Size

Alpha Vantage supports two output sizes for this endpoint:

```csharp
OutputSize.Compact // "compact"
OutputSize.Full    // "full"
```

`compact` returns the latest subset of daily data. `full` requests the full available daily history from Alpha Vantage.

## Data Model

`GetForexHistoricalAsync` returns a `List<ForexData>`.

```csharp
public class ForexData
{
    public DateTime Date { get; set; }
    public decimal Open { get; set; }
    public decimal Close { get; set; }
    public decimal High { get; set; }
    public decimal Low { get; set; }
    public long? Volume { get; set; }
}
```

Forex responses from Alpha Vantage do not include volume, so `Volume` is currently set to `0`.

## Error Handling

If Alpha Vantage returns an API error response, the wrapper throws an exception with the API message:

```csharp
try
{
    var candles = await alphaVantage.GetForexHistoricalAsync("INVALID", "USD", OutputSize.Compact);
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}
```

If no data is returned from the HTTP request, the wrapper throws:

```text
API Error: No data returned from Alpha Vantage.
```

## Custom HttpClient

You can provide an `HttpClient` when you need custom handlers, logging, timeouts, proxies, or deterministic tests:

```csharp
using var httpClient = new HttpClient
{
    Timeout = TimeSpan.FromSeconds(30)
};

var alphaVantage = new AlphaVantage("YOUR_ALPHA_VANTAGE_API_KEY", httpClient);
```

## Development

Restore and build the solution:

```powershell
dotnet build Yanga.Module.AlphaVantageApi.sln
```

Run the test suite:

```powershell
dotnet test Yanga.Module.AlphaVantageApi.sln
```

The tests include deterministic JSON parsing checks and wrapper-level HTTP tests using a fake `HttpClient`.

## License

This project is licensed under the MIT License.
