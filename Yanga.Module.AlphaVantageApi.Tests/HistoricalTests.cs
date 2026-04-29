namespace Yanga.Module.AlphaVantageApi.Tests
{
    public class HistoricalTests
    {
        private const string DailyForexJson = @"
            {
                ""Meta Data"": {
                    ""1. Information"": ""Forex Daily Prices (open, high, low, close)"",
                    ""2. From Symbol"": ""EUR"",
                    ""3. To Symbol"": ""USD"",
                    ""4. Output Size"": ""Compact"",
                    ""5. Last Refreshed"": ""2023-11-20 19:20:00"",
                    ""6. Time Zone"": ""UTC""
                },
                ""Time Series FX (Daily)"": {
                    ""2023-11-20"": {
                        ""1. open"": ""1.09120000"",
                        ""2. high"": ""1.09520000"",
                        ""3. low"": ""1.08960000"",
                        ""4. close"": ""1.09410000""
                    },
                    ""2023-11-17"": {
                        ""1. open"": ""1.08490000"",
                        ""2. high"": ""1.09140000"",
                        ""3. low"": ""1.08240000"",
                        ""4. close"": ""1.09110000""
                    }
                }
            }";

        private const string EmptyTimeSeriesJson = @"
            {
                ""Meta Data"": {
                    ""1. Information"": ""Forex Daily Prices (open, high, low, close)"",
                    ""2. From Symbol"": ""EUR"",
                    ""3. To Symbol"": ""USD"",
                    ""4. Output Size"": ""Compact"",
                    ""5. Last Refreshed"": ""2023-11-20 19:20:00"",
                    ""6. Time Zone"": ""UTC""
                },
                ""Time Series FX (Daily)"": {
                }
            }";

        private const string MissingTimeSeriesJson = @"
            {
                ""Meta Data"": {
                    ""1. Information"": ""Forex Daily Prices (open, high, low, close)"",
                    ""2. From Symbol"": ""EUR"",
                    ""3. To Symbol"": ""USD"",
                    ""4. Output Size"": ""Compact"",
                    ""5. Last Refreshed"": ""2023-11-20 19:20:00"",
                    ""6. Time Zone"": ""UTC""
                }
            }";

        [Fact]
        public async Task InvalidSymbolTest()
        {
            string apiKey = "ALPHAVANTAGE-API-KEY";

            var exception = await Assert.ThrowsAsync<Exception>(async () =>
                await new AlphaVantage(apiKey).GetForexHistoricalAsync("invalidSymbol", "EUR", "full"));

            Assert.Contains("Invalid", exception.Message);
        }

        [Fact]
        public void AlphaVantageToCandleReturnsDailyCandles()
        {
            var candles = AlphaVantageHistoric.AlphaVantageToCandle(DailyForexJson);

            Assert.Equal(2, candles.Count);
        }

        [Fact]
        public void AlphaVantageToCandleMapsLatestDailyCandle()
        {
            var candles = AlphaVantageHistoric.AlphaVantageToCandle(DailyForexJson);
            var candle = candles.First();

            Assert.Equal(new DateTime(2023, 11, 20), candle.Date);
            Assert.Equal(1.09120000m, candle.Open);
            Assert.Equal(1.09520000m, candle.High);
            Assert.Equal(1.08960000m, candle.Low);
            Assert.Equal(1.09410000m, candle.Close);
            Assert.Equal(0, candle.Volume);
        }

        [Fact]
        public void AlphaVantageToCandlePreservesApiOrdering()
        {
            var candles = AlphaVantageHistoric.AlphaVantageToCandle(DailyForexJson);

            Assert.Equal(new DateTime(2023, 11, 20), candles.First().Date);
            Assert.Equal(new DateTime(2023, 11, 17), candles.Last().Date);
        }

        [Fact]
        public void AlphaVantageToCandleMapsOlderDailyCandle()
        {
            var candles = AlphaVantageHistoric.AlphaVantageToCandle(DailyForexJson);
            var candle = candles.Last();

            Assert.Equal(new DateTime(2023, 11, 17), candle.Date);
            Assert.Equal(1.08490000m, candle.Open);
            Assert.Equal(1.09140000m, candle.High);
            Assert.Equal(1.08240000m, candle.Low);
            Assert.Equal(1.09110000m, candle.Close);
            Assert.Equal(0, candle.Volume);
        }

        [Fact]
        public void AlphaVantageToCandleReturnsEmptyListForEmptyTimeSeries()
        {
            var candles = AlphaVantageHistoric.AlphaVantageToCandle(EmptyTimeSeriesJson);

            Assert.Empty(candles);
        }

        [Fact]
        public void AlphaVantageToCandleThrowsWhenTimeSeriesIsMissing()
        {
            Assert.Throws<KeyNotFoundException>(() =>
                AlphaVantageHistoric.AlphaVantageToCandle(MissingTimeSeriesJson));
        }

        [Fact]
        public void AlphaVantageToCandleThrowsWhenJsonIsInvalid()
        {
            var exception = Record.Exception(() =>
                AlphaVantageHistoric.AlphaVantageToCandle("{ invalid json }"));

            Assert.IsAssignableFrom<System.Text.Json.JsonException>(exception);
        }

        [Fact]
        public void OutputSizeReturnsAlphaVantageValues()
        {
            Assert.Equal("compact", OutputSize.Compact);
            Assert.Equal("full", OutputSize.Full);
        }

        [Fact]
        public async Task GetForexHistoricalAsyncMapsSuccessfulResponse()
        {
            var client = CreateClient(DailyForexJson);
            var candles = await new AlphaVantage("demo-key", client)
                .GetForexHistoricalAsync("EUR", "USD", OutputSize.Compact);

            Assert.Equal(2, candles.Count);
            Assert.Equal(1.09410000m, candles.First().Close);
        }

        [Fact]
        public async Task GetForexHistoricalAsyncBuildsAlphaVantageQuery()
        {
            Uri? requestedUri = null;
            var client = CreateClient(DailyForexJson, request => requestedUri = request.RequestUri);

            await new AlphaVantage("demo-key", client)
                .GetForexHistoricalAsync("EUR", "USD", OutputSize.Full);

            Assert.NotNull(requestedUri);
            Assert.Equal("www.alphavantage.co", requestedUri.Host);
            Assert.Contains("function=FX_DAILY", requestedUri.Query);
            Assert.Contains("from_symbol=EUR", requestedUri.Query);
            Assert.Contains("to_symbol=USD", requestedUri.Query);
            Assert.Contains("outputsize=full", requestedUri.Query);
            Assert.Contains("datatype=json", requestedUri.Query);
            Assert.Contains("apikey=demo-key", requestedUri.Query);
        }

        [Fact]
        public async Task GetForexHistoricalAsyncDefaultsToCompactOutputSize()
        {
            Uri? requestedUri = null;
            var client = CreateClient(DailyForexJson, request => requestedUri = request.RequestUri);

            await new AlphaVantage("demo-key", client)
                .GetForexHistoricalAsync("EUR", "USD", null!);

            Assert.NotNull(requestedUri);
            Assert.Contains("outputsize=compact", requestedUri.Query);
        }

        [Fact]
        public async Task GetForexHistoricalAsyncThrowsApiErrorMessage()
        {
            const string apiError = @"{ ""Error Message"": ""Invalid API call."" }";
            var client = CreateClient(apiError);

            var exception = await Assert.ThrowsAsync<Exception>(() =>
                new AlphaVantage("demo-key", client).GetForexHistoricalAsync("invalidSymbol", "USD", OutputSize.Compact));

            Assert.Equal("API Error: Invalid API call.", exception.Message);
        }

        [Fact]
        public async Task GetForexHistoricalAsyncThrowsWhenHttpRequestFails()
        {
            var client = CreateClient("server error", statusCode: System.Net.HttpStatusCode.InternalServerError);

            var exception = await Assert.ThrowsAsync<Exception>(() =>
                new AlphaVantage("demo-key", client).GetForexHistoricalAsync("EUR", "USD", OutputSize.Compact));

            Assert.Equal("API Error: No data returned from Alpha Vantage.", exception.Message);
        }

        private static HttpClient CreateClient(
            string responseBody,
            Action<HttpRequestMessage>? onRequest = null,
            System.Net.HttpStatusCode statusCode = System.Net.HttpStatusCode.OK)
        {
            var handler = new TestHttpMessageHandler(request =>
            {
                onRequest?.Invoke(request);
                return new HttpResponseMessage(statusCode)
                {
                    Content = new StringContent(responseBody)
                };
            });

            return new HttpClient(handler);
        }

        private sealed class TestHttpMessageHandler : HttpMessageHandler
        {
            private readonly Func<HttpRequestMessage, HttpResponseMessage> send;

            public TestHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> send)
            {
                this.send = send;
            }

            protected override Task<HttpResponseMessage> SendAsync(
                HttpRequestMessage request,
                CancellationToken cancellationToken)
            {
                return Task.FromResult(send(request));
            }
        }
    }
}
