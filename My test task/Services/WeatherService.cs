using System.Text.Json;
using My_test_task.Models;

namespace My_test_task.Services
{
    public class WeatherService
    {
        private readonly string _apiKey;
        private readonly HttpClient _httpClient;

        public WeatherService(IConfiguration configuration)
        {
            _apiKey = configuration["WeatherApiKey"];
            _httpClient = new HttpClient();
        }

        public async Task<WeatherModel?> GetWeatherAsync(string city)
        {
            string url = $"https://api.openweathermap.org/data/2.5/weather?q={city}&appid={_apiKey}&units=metric";

            try
            {
                var response = await _httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Помилка API: {response.StatusCode}");
                    return null; 
                }

                string responseContent = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(responseContent);
                var root = doc.RootElement;

                return new WeatherModel
                {
                    City = root.GetProperty("name").GetString(),
                    Temperature = root.GetProperty("main").GetProperty("temp").GetDouble(),
                    MinTemperature = root.GetProperty("main").GetProperty("temp_min").GetDouble(),
                    MaxTemperature = root.GetProperty("main").GetProperty("temp_max").GetDouble(),
                    Description = root.GetProperty("weather").EnumerateArray().First().GetProperty("description").GetString(),
                    WillRain = root.GetProperty("weather").EnumerateArray().First().GetProperty("main").GetString().ToLower().Contains("rain")
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine("Помилка під час отримання погоди: " + ex.Message);
                return null;
            }
        }
    }
}
