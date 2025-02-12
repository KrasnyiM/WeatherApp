using Microsoft.AspNetCore.Mvc;
using My_test_task.Services;

namespace My_test_task.Controllers
{
    public class WeatherController : Controller
    {
        private readonly WeatherService _weatherService;
        private const string CookieName = "LastCity";

        public WeatherController(WeatherService weatherService)
        {
            _weatherService = weatherService;
        }

        public async Task<IActionResult> Index()
        {
            string city = Request.Cookies[CookieName] ?? "Kyiv";
            Console.WriteLine("Місто з Cookies: " + city);

            var weather = await _weatherService.GetWeatherAsync(city);
            return View(weather);
        }

        [HttpPost]
        public async Task<IActionResult> Search(string city)
        {
            Console.WriteLine("Отримане місто: " + city);

            if (string.IsNullOrEmpty(city))
            {
                TempData["Error"] = "Будь ласка, введіть місто!";
                return RedirectToAction("Index");
            }

            var weather = await _weatherService.GetWeatherAsync(city);

            if (weather == null)
            {
                TempData["Error"] = "Місто не знайдено! Переконайтесь, що ви ввели правильну назву.";
                return RedirectToAction("Index");
            }

            Response.Cookies.Append("LastCity", city, new CookieOptions
            {
                Expires = DateTime.UtcNow.AddDays(30),
                HttpOnly = false
            });

            return View("Index", weather);
        }
    }
}
