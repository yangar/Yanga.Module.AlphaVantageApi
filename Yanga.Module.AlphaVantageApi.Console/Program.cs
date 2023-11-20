using Yanga.Module.AlphaVantageApi;

var data = await new AlphaVantage("ALPHAVANTAGE-API-KEY").GetHistoricalAsync("EUR", "THB", OutputSize.Compact);
foreach (var item in data)
{
    Console.WriteLine($"DateTime: {item.Date}, Open: {item.Open}, High: {item.High}, Low: {item.Low}, Close: {item.Close}, Volume: {item.Volume}");
}
