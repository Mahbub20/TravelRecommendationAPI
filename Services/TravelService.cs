using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TravelRecommendationApi.Models;

namespace TravelRecommendationApi.Services
{
    public class TravelService
    {
        private readonly DistrictService _districtService;
        private readonly WeatherService _weatherService;
        public TravelService(DistrictService districtService, WeatherService weatherService)
        {
            _districtService = districtService;
            _weatherService = weatherService;
        }

        public async Task<TravelResponse> GetRecommendationAsync(TravelRequest req)
        {
            var dest = _districtService.GetDistrictByName(req.DestinationDistrict);
            if (dest == null)
            {
                return new TravelResponse { Result = "Not Found", Reason = "Invalid Destination District" };
            }

            var currentTemp = await _weatherService.GetTempAt2PMOnDateAsync(req.CurrentLat, req.CurrentLong, req.TravelDate);
            var destTemp = await _weatherService.GetTempAt2PMOnDateAsync(Convert.ToDouble(dest.Lat), Convert.ToDouble(dest.Long), req.TravelDate);

            var currentPm = await _weatherService.GetPm25Async(req.CurrentLat, req.CurrentLong);
            var destPm = await _weatherService.GetPm25Async(Convert.ToDouble(dest.Lat), Convert.ToDouble(dest.Long));

            var tempDiff = currentTemp - destTemp;
            var airDiff = currentPm - destPm;

            if (tempDiff > 0 && airDiff > 0)
            {
                return new TravelResponse
                {
                    Result = "Recommended",
                    Reason = $"Your destination is {Math.Abs(tempDiff):F1}°C cooler and has significantly better air quality. Enjoy your trip!"
                };
            }

            return new TravelResponse
            {
                Result = "Not Recommended",
                Reason = $"Your destination is {Math.Abs(tempDiff):F1}°C hotter and has worse air quality than your current location. It’s better to stay where you are."
            };
        }
    }
}