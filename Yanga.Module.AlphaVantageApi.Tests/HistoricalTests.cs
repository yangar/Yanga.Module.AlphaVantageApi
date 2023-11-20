namespace TestProject1Yanga.Module.AlphaVantageApi.Tests
{
    public class HistoricalTests
    {
        private readonly Action<string> Write;

        [Fact]
        public async Task InvalidSymbolTest()
        {
            string apiKey = "ALPHAVANTAGE-API-KEY";
            AlphaVantage apiWrapper = new AlphaVantage(apiKey);

            var exception = await Assert.ThrowsAsync<Exception>(async () =>
                await new AlphaVantage(apiKey).GetHistoricalAsync("invalidSymbol", "EUR"));

            //Write(exception.ToString());

            Assert.Contains("Invalid", exception.InnerException.Message);
        }
    }
}