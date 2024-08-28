using Moq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging.Abstractions;
using WeatherWorkerService;

public class WeatherServiceTests
{
    private HttpClient _httpClient;
    private readonly IOptions<WeatherApiOptions> _options;
    private WeatherService _weatherService;

    public WeatherServiceTests()
    {
        _options = Options.Create(new WeatherApiOptions
        {
            ApiKey = "7f5022d40e81444ff4d64c133ee0f566",
            BaseUrl = "http://api.openweathermap.org/data/2.5/weather"
        });
    }

    [Fact]
    public async Task GetWeatherAsync_ReturnsWeatherData()
    {
        // Arrange
        var city = "Cape Town";
        var expectedResponse = new WeatherData
        {
            Main = new Main { Temp = 32, TempC = 0, Humidity = 60, Pressure = 1012 },
            Wind = new Wind { Speed = 5 },
            Name = city
        };
        var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = JsonContent.Create(expectedResponse)
        };

        var httpMessageHandlerMock = new HttpMessageHandlerMock((request, cancellationToken) => Task.FromResult(httpResponse));
        _httpClient = new HttpClient(httpMessageHandlerMock);
        _weatherService = new WeatherService(_httpClient, _options);

        // Act
        var result = await _weatherService.GetWeatherAsync(city);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedResponse.Main.Temp, result.Main.Temp);
        Assert.Equal(expectedResponse.Main.TempC, result.Main.TempC);
        Assert.Equal(expectedResponse.Main.Humidity, result.Main.Humidity);
        Assert.Equal(expectedResponse.Main.Pressure, result.Main.Pressure);
        Assert.Equal(expectedResponse.Wind.Speed, result.Wind.Speed);
        Assert.Equal(expectedResponse.Name, result.Name);
    }

    [Fact]
    public async Task GetWeatherAsync_HandlesHttpRequestException()
    {
        // Arrange
        var city = "Cape Town";
        var httpMessageHandlerMock = new HttpMessageHandlerMock((request, cancellationToken) => throw new HttpRequestException());
        _httpClient = new HttpClient(httpMessageHandlerMock);
        _weatherService = new WeatherService(_httpClient, _options);

        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(() => _weatherService.GetWeatherAsync(city));
    }
}