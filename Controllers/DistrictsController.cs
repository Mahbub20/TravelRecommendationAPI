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

                var tasks = districts.Select(async d => new
                {
                    d.Name,
                    Weather = await _weatherService.Get7DayWeatherAsync(d.Lat, d.Long)
                });

                var results = await Task.WhenAll(tasks);

                var top10 = results
                    .OrderBy(x => x.Weather.AverageTemp)
                    .ThenBy(x => x.Weather.AverageAirQuality)
                    .Take(10)
                    .Select(x => new
                    {
                        x.Name,
                        x.Weather.AverageTemp,
                        x.Weather.AverageAirQuality
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