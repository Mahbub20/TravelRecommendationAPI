using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using TravelRecommendationApi.Models;
using TravelRecommendationApi.Utils;

namespace TravelRecommendationApi.Services
{
    public class WeatherService
    {
        private readonly HttpHelper _http;
        private readonly IMemoryCache _cache;
        private readonly TimeSpan cacheDuration = TimeSpan.FromMinutes(30);

        public WeatherService(HttpHelper http, IMemoryCache cache)
        {
            _http = http;
            _cache = cache;
        }

        public async Task<WeatherData> Get7DayWeatherAsync(double lat, double lon)
        {
            string cacheKey = $"weather_7d_{lat}_{lon}";
            if (_cache.TryGetValue(cacheKey, out WeatherData cached)) return cached;

            var data = await Fetch7DayWeatherAsync(lat, lon);
            _cache.Set(cacheKey, data, cacheDuration);
            return data;
        }

        public async Task<WeatherData> Fetch7DayWeatherAsync(double lat, double lon)
        {
            string weatherUrl = $"https://api.open-meteo.com/v1/forecast?latitude={lat}&longitude={lon}&hourly=temperature_2m&timezone=auto&forecast_days=7";
            var weather = await _http.GetAsync<JsonElement>(weatherUrl);

            string airUrl = $"https://air-quality-api.open-meteo.com/v1/air-quality?latitude={lat}&longitude={lon}&hourly=pm2_5&timezone=auto&forecast_days=7";
            var air = await _http.GetAsync<JsonElement>(airUrl);

            var times = weather.GetProperty("hourly").GetProperty("time").EnumerateArray().Select(t => t.GetString()).ToList();
            var temps = weather.GetProperty("hourly").GetProperty("temperature_2m")
            .EnumerateArray()
            .Select(t => t.ValueKind == JsonValueKind.Number ? t.GetDouble() : double.NaN)
            .Where(v => !double.IsNaN(v))
            .ToList();

            var tempsAt2PM = new List<double>();
            for (int i = 0; i < times.Count; i++)
            {
                if (times[i]?.EndsWith("T14:00") ?? false)
                    tempsAt2PM.Add(temps[i]);
            }
            double avgTemp2PM = tempsAt2PM.Count > 0 ? tempsAt2PM.Average() : 0.0;
            var pm25 = air.GetProperty("hourly").GetProperty("pm2_5")
            .EnumerateArray()
            .Select(t => t.ValueKind == JsonValueKind.Number ? t.GetDouble() : double.NaN)
            .Where(v => !double.IsNaN(v))
            .ToList();
            double avgPm25 = pm25.Count > 0 ? pm25.Average() : 0.0;

            return new WeatherData { AverageTemp = avgTemp2PM, AverageAirQuality = avgPm25 };
        }

        public async Task<double> GetTempAt2PMOnDateAsync(double lat, double lon, DateTime date)
        {
            string weatherUrl = $"https://api.open-meteo.com/v1/forecast?latitude={lat}&longitude={lon}&hourly=temperature_2m&timezone=auto&forecast_days=7";
            var weather = await _http.GetAsync<JsonElement>(weatherUrl);

            var times = weather.GetProperty("hourly")
            .GetProperty("time")
            .EnumerateArray()
            .Select(t => DateTime
            .Parse(t.GetString()))
            .ToList();
            var temps = weather.GetProperty("hourly").GetProperty("temperature_2m")
            .EnumerateArray()
            .Select(t => t.ValueKind == JsonValueKind.Number ? t.GetDouble() : double.NaN)
            .Where(v => !double.IsNaN(v))
            .ToList();

            DateTime targetLocal = date.Date.AddHours(14); // 2 PM local time
            int index = times.FindIndex(t => Math.Abs((t - targetLocal).TotalHours) < 0.5);
            var resultTemp = index != -1 ? temps[index] : double.NaN;
            return resultTemp;
        }

        public async Task<double> GetPm25Async(double lat, double lon)
        {
            string airUrl = $"https://air-quality-api.open-meteo.com/v1/air-quality?latitude={lat}&longitude={lon}&hourly=pm2_5&timezone=auto&forecast_days=7";
            var air = await _http.GetAsync<JsonElement>(airUrl);
            var pmValuesNumeric = air.GetProperty("hourly")
                                     .GetProperty("pm2_5")
                                     .EnumerateArray()
                                     .Where(t => t.ValueKind == JsonValueKind.Number) // keep only numbers
                                     .Select(t => t.GetDouble())
                                     .ToList();

            if (pmValuesNumeric.Count == 0)
            {
                Console.WriteLine("No numeric PM2.5 data available for this location.");
                return double.NaN; // or some default value
            }

            var resultAir = pmValuesNumeric.Average();
            return resultAir;

        }
    }
}