using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using TravelRecommendationApi.Properties.Models;
using TravelRecommendationApi.Utils;

namespace TravelRecommendationApi.Services
{
    public class WeatherService
    {
        private readonly HttpHelper _http;

        public WeatherService(HttpHelper http)
        {
            _http = http;
        }

        public async Task<WeatherData> Get7DayWeatherAsync(double lat, double lon)
        {
            string weatherUrl = $"https://api.open-meteo.com/v1/forecast?latitude={lat}&longitude={lon}&hourly=temperature_2m&timezone=auto&forecast_days=7";
            var weather = await _http.GetAsync<JsonElement>(weatherUrl);

            string airUrl = $"https://air-quality-api.open-meteo.com/v1/air-quality?latitude={lat}&longitude={lon}&hourly=pm2_5&timezone=auto&forecast_days=7";
            var air = await _http.GetAsync<JsonElement>(airUrl);

            var times = weather.GetProperty("hourly").GetProperty("time").EnumerateArray().Select(t => t.GetString()).ToList();
            var temps = weather.GetProperty("hourly").GetProperty("temperature_2m").EnumerateArray().Select(t => t.GetDouble()).ToList();

            var tempsAt2PM = new List<double>();
            for (int i = 0; i < times.Count; i++)
            {
                if (times[i]?.EndsWith("T14:00") ?? false)
                    tempsAt2PM.Add(temps[i]);
            }
            double avgTemp2PM = tempsAt2PM.Average();

            var pm25 = air.GetProperty("hourly").GetProperty("pm2_5").EnumerateArray().Select(t => t.GetDouble()).ToList();
            double avgPm25 = pm25.Average();

            return new WeatherData { AverageTemp = avgTemp2PM, AverageAirQuality = avgPm25 };
        }

        public async Task<double> GetTempAt2PMOnDateAsync(double lat, double lon, DateTime date)
        {
            string weatherUrl = $"https://api.open-meteo.com/v1/forecast?latitude={lat}&longitude={lon}&hourly=temperature_2m&timezone=auto&forecast_days=7";
            var weather = await _http.GetAsync<JsonElement>(weatherUrl);

            var times = weather.GetProperty("hourly").GetProperty("time").EnumerateArray().Select(t => t.GetString()).ToList();
            var temps = weather.GetProperty("hourly").GetProperty("temperature_2m").EnumerateArray().Select(t => t.GetDouble()).ToList();

            string target = date.ToString("yyyy-MM-dd'T'14:00");
            int index = times.FindIndex(t => t == target);
            return index != -1 ? temps[index] : double.NaN;
        }

        public async Task<double> GetPm25Async(double lat, double lon)
        {
            string airUrl = $"https://air-quality-api.open-meteo.com/v1/air-quality?latitude={lat}&longitude={lon}&hourly=pm2_5&timezone=auto&forecast_days=7";
            var air = await _http.GetAsync<JsonElement>(airUrl);
            return air.GetProperty("hourly").GetProperty("pm2_5").EnumerateArray().Select(t => t.GetDouble()).Average();
        }
    }
}