using System;
using System.Collections.Generic;
using System.Text.Json;

public class ForexData
{
    public DateTime Date { get; set; }
    public double Open { get; set; }
    public double Close { get; set; }
    public double High { get; set; }
    public double Low { get; set; }
    public double Volume { get; set; }
}

public static class Historic
{
    public static List<ForexData> AlphaVantageToCandle(string jsonData)
    {
        List<ForexData> result = new List<ForexData>();

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
                    Open = Convert.ToDouble(values.GetProperty("1. open").GetString()),
                    Close = Convert.ToDouble(values.GetProperty("4. close").GetString()),
                    High = Convert.ToDouble(values.GetProperty("2. high").GetString()),
                    Low = Convert.ToDouble(values.GetProperty("3. low").GetString()),
                    Volume = 0 // You can set the volume to a default value or retrieve it if available
                };

                result.Add(forexData);
            }
        }

        return result;
    }
}