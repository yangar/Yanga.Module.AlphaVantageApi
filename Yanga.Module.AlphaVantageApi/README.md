# Yanga.Module.AlphaVantageApi

This is a .NET wrapper for the Alpha Vantage API, providing an easy way to retrieve historical Forex data in a convenient format. The wrapper includes a method to get historical daily Forex prices and converts the response to a more user-friendly format, based on .NET 6

## Features
* Get quotes (to do)
* Get historical data

## v1.0 initial release
* Retreive historic

## Supported Platforms
* .NET Core 6.0+

## How To Install
You can find the package through Nuget

    PM> Install-Package Yanga.Module.AlphaVantageApi

## How To Use

### Add reference

    using Yanga.Module.AlphaVantageApi;

### Get stock quotes
    To do

### Get historical data for a stock
    // You should be able to query data from various markets including US, HK, TW
    // The startTime & endTime here defaults to EST timezone
    var history = await new AlphaVantage("AlphaVantage-Api-Key").GetHistoricalAsync("EUR", "THB", OutputSize.Compact);

    foreach (var candle in history)
    {
        Console.WriteLine($"DateTime: {candle.DateTime}, Open: {candle.Open}, High: {candle.High}, Low: {candle.Low}, Close: {candle.Close}, Volume: {candle.Volume}");
    }