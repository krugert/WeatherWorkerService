using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Polly;
using System.Net.Http;
using WeatherWorkerService;

public interface IWeatherService
{
    Task<WeatherData> GetWeatherAsync(string city);
}

public class WeatherService : IWeatherService
{
    private readonly HttpClient _httpClient;
    private readonly WeatherApiOptions _options;
    private readonly AsyncPolicy<HttpResponseMessage> _retryPolicy;

    public WeatherService(HttpClient httpClient, IOptions<WeatherApiOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    public async Task<WeatherData> GetWeatherAsync(string city)
    {
        WeatherData? weatherData = null;

        var response = await _httpClient.GetAsync($"{_options.BaseUrl}?q={city}&appid={_options.ApiKey}");
        response.EnsureSuccessStatusCode();
        weatherData = await response.Content.ReadFromJsonAsync<WeatherData>();

        return weatherData;
    }
}

public class WeatherData
{
    public Main Main { get; set; }
    public Wind Wind { get; set; }
    public string Name { get; set; }
}

public class Main
{
    private double _tempC;

    public double Temp { get; set; }
    public double TempC
    {
        get
        {
            _tempC = 0.5556 * (Temp - 32.0);

            return _tempC;
        }
        set
        {
            if (_tempC != value)
            {
                _tempC = value;
            }
        }
    }
    public int Pressure { get; set; }
    public int Humidity { get; set; }
}

public class Wind
{
    public double Speed { get; set; }
}
