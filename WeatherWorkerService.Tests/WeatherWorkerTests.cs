using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using WeatherWorkerService;

public class WeatherWorkerTests
{
    private readonly IOptions<WeatherApiOptions> _options;
    private readonly Mock<IWeatherService> _weatherServiceMock;
    private readonly Mock<ILogger<WeatherWorker>> _loggerMock;
    private readonly WeatherWorker _weatherWorker;

    public WeatherWorkerTests()
    {
        _options = Options.Create(new WeatherApiOptions
        {
            ApiKey = "7f5022d40e81444ff4d64c133ee0f566",
            BaseUrl = "http://api.openweathermap.org/data/2.5/weather",
            Cities = [ "Cape Town" ]
        });

        _weatherServiceMock = new Mock<IWeatherService>();
        _loggerMock = new Mock<ILogger<WeatherWorker>>();

        // Ensure all dependencies are passed to the WeatherWorker constructor
        _weatherWorker = new WeatherWorker(_loggerMock.Object, _options, _weatherServiceMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_LogsWeatherData()
    {
        // Arrange
        var weatherData = new WeatherData
        {
            Main = new Main { Temp = 32, Humidity = 60, Pressure = 1012 },
            Wind = new Wind { Speed = 5 },
            Name = "Cape Town"
        };
        _weatherServiceMock
            .Setup(s => s.GetWeatherAsync(It.IsAny<string>()))
            .ReturnsAsync(weatherData);

        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(1000); // Cancel after 1 second to stop the loop

        // Act
        await _weatherWorker.StartAsync(cancellationTokenSource.Token);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Weather in Cape Town")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.AtLeastOnce);
    }
}