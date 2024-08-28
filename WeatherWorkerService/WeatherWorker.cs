using Microsoft.Extensions.Options;
using WeatherWorkerService;

public class WeatherWorker : BackgroundService
{
    private readonly ILogger<WeatherWorker> _logger;
    private readonly IOptions<WeatherApiOptions> _options;
    private readonly IWeatherService _weatherService;

    public WeatherWorker(ILogger<WeatherWorker> logger, IOptions<WeatherApiOptions> options, IWeatherService weatherService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _weatherService = weatherService ?? throw new ArgumentNullException(nameof(weatherService));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            foreach (string _city in _options.Value.Cities)
            {
                try
                {
                    var weatherData = await _weatherService.GetWeatherAsync(_city);
                    _logger.LogInformation($"Weather in {weatherData.Name}: {Math.Round(weatherData.Main.TempC, 0)}°C, {weatherData.Main.Humidity}% humidity, {weatherData.Main.Pressure} hPa, Wind {weatherData.Wind.Speed} m/s");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error fetching weather data for {_city}", _city);
                }
            }
            await Task.Delay(10000, stoppingToken); // Delay for 10 seconds
        }
    }

    /*
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var weatherData = await _weatherService.GetWeatherAsync("Cape Town");
                _logger.LogInformation($"Weather in {weatherData.Name}: {Math.Max(weatherData.Main.TempC, 0)}°C, {weatherData.Main.Humidity}% humidity, {weatherData.Main.Pressure} hPa, Wind {weatherData.Wind.Speed} m/s");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching weather data");
            }

            await Task.Delay(10000, stoppingToken); // Delay for 10 seconds
        }
    }
    */
}