using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TravelRecommendationApi.Services;

namespace TravelRecommendationApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DistrictsController : ControllerBase
    {
        private readonly DistrictService _districtService;
        private readonly WeatherService _weatherService;

        public DistrictsController(DistrictService districtService, WeatherService weatherService)
        {
            _districtService = districtService;
            _weatherService = weatherService;
        }

        [HttpGet("top10")]
        public async Task<IActionResult> GetTop10()
        {
            try
            {
                var districts = _districtService.GetAllDistricts();
                var results = new List<object>();
                var throttler = new SemaphoreSlim(5); // limit 5 concurrent API calls

                var tasks = districts.Select(async d =>
                {
                    await throttler.WaitAsync();
                    try
                    {
                        double lat = double.TryParse(d.Lat, out var latVal) ? latVal : 0;
                        double lon = double.TryParse(d.Long, out var lonVal) ? lonVal : 0;

                        if (lat == 0 || lon == 0) return;

                        var weather = await _weatherService.Get7DayWeatherAsync(lat, lon);
                        lock (results)
                        {
                            results.Add(new { d.Name, Weather = weather });
                        }
                    }
                    catch
                    {
                        // skip failed district quietly
                    }
                    finally
                    {
                        throttler.Release();
                    }
                });

                await Task.WhenAll(tasks);

                var top10 = results
                    .OrderBy(x => ((dynamic)x).Weather.AverageTemp)
                    .ThenBy(x => ((dynamic)x).Weather.AverageAirQuality)
                    .Take(10)
                    .Select(x => new
                    {
                        ((dynamic)x).Name,
                        ((dynamic)x).Weather.AverageTemp,
                        ((dynamic)x).Weather.AverageAirQuality
                    });

                return Ok(top10);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Result = "Error", Reason = ex.Message });
            }
        }

    }
}