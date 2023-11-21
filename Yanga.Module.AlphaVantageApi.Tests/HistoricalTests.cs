namespace Yanga.Module.AlphaVantageApi.Tests
{
    public class HistoricalTests
    {
        private readonly Action<string> Write;

        [Fact]
        public async Task InvalidSymbolTest()
        {
            string apiKey = "ALPHAVANTAGE-API-KEY";
            AlphaVantage apiWrapper = new(apiKey);

            var exception = await Assert.ThrowsAsync<Exception>(async () =>
                await new AlphaVantage(apiKey).GetForexHistoricalAsync("invalidSymbol", "EUR", "full"));

            //Write(exception.ToString());

            Assert.Contains("Invalid", exception.InnerException.Message);
        }
    }
}