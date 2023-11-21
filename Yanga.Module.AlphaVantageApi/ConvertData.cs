using System;
using System.Collections.Generic;
using System.Text.Json;

public class ForexData
{
    public DateTime Date { get; set; }
    public decimal Open { get; set; }
    public decimal Close { get; set; }
    public decimal High { get; set; }
    public decimal Low { get; set; }
    public decimal Volume { get; set; }
}

public static class Historic
{
    public static List<ForexData> AlphaVantageToCandle(string jsonData)
    {
        List<ForexData> result = new();

        // Parse the JSON data
        JsonDocument document = JsonDocument.Parse(jsonData);
        JsonElement root = document.RootElement;

        // Extract meta data and time series
        JsonElement metaData = root.GetProperty("Meta Data");
        JsonElement timeSeries = root.GetProperty("Time Series FX (Daily)");

        if (!metaData.Equals(JsonValueKind.Undefined) && !timeSeries.Equals(JsonValueKind.Undefined))
        {
            string fromSymbol = metaData.GetProperty("2. From Symbol").GetString();
            string toSymbol = metaData.GetProperty("3. To Symbol").GetString();

            foreach (var entry in timeSeries.EnumerateObject())
            {
                DateTime date = DateTime.Parse(entry.Name);
                var values = entry.Value;

                ForexData forexData = new ForexData
                {
                    Date = date,
                    Open = Convert.ToDecimal(values.GetProperty("1. open").GetString()),
                    Close = Convert.ToDecimal(values.GetProperty("4. close").GetString()),
                    High = Convert.ToDecimal(values.GetProperty("2. high").GetString()),
                    Low = Convert.ToDecimal(values.GetProperty("3. low").GetString()),
                    Volume = 0 // You can set the volume to a default value or retrieve it if available
                };

                result.Add(forexData);
            }
        }

        return result;
    }
}