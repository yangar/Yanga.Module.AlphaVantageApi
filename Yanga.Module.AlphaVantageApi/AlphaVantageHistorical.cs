using System.Text.Json;
using Yanga.Module.AlphaVantageApi;

public class AlphaVantage
{
    private readonly string apiKey;
    private readonly HttpClient httpClient;

    public AlphaVantage(string apiKey)
    {
        this.apiKey = apiKey;
        this.httpClient = new HttpClient();
    }

    public async Task<List<ForexData>> GetHistoricalAsync(string fromSymbol, string toSymbol, string outputSize = "compact", string dataType = "json")
    {
        var data = await GetDataAsync(fromSymbol, toSymbol, OutputSize.Full, dataType);

        // Parse the JSON data
        JsonDocument document = JsonDocument.Parse(data);
        JsonElement root = document.RootElement;
        if (root.TryGetProperty("Error Message", out JsonElement error))
        {
            throw new Exception($"API Error: {error.GetString()}");
        }
        /*if (!error.Equals(JsonValueKind.Undefined))
        {
           throw new Exception($"API Error: {error.GetString()}");
        }*/

        // Extract meta data and time series
        JsonElement metaData = root.GetProperty("Meta Data");
        return Historic.AlphaVantageToCandle(data);
    }
    private async Task<string> GetDataAsync(string fromSymbol, string toSymbol, string outputSize = "compact", string dataType = "json")
    {
        string apiUrl = $"https://www.alphavantage.co/query?function=FX_DAILY&from_symbol={fromSymbol}&to_symbol={toSymbol}&outputsize={outputSize}&datatype={dataType}&apikey={apiKey}";

        try
        {
            HttpResponseMessage response = await httpClient.GetAsync(apiUrl);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            else
            {
                // Handle the error response here
                Console.WriteLine($"Error: {response.StatusCode} - {response.ReasonPhrase}");
                return null;
            }
        }
        catch (Exception ex)
        {
            // Handle any exceptions that may occur during the API call
            Console.WriteLine($"Exception: {ex.Message}");
            return null;
        }
    }
}