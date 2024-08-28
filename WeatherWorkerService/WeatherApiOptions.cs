namespace WeatherWorkerService
{
    public class WeatherApiOptions
    {
        public string ApiKey { get; set; }
        public string BaseUrl { get; set; }
        public int RefreshRateInMinutes { get; set; }
        public List<string> Cities { get; set; }
        public int NumberOfRetries { get; set; }
        public int NumberOfSeconds { get; set; }
    }
}
