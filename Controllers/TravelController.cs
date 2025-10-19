using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TravelRecommendationApi.Models;
using TravelRecommendationApi.Services;

namespace TravelRecommendationApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TravelController : ControllerBase
    {
        private readonly TravelService _travelService;
        public TravelController(TravelService travelService)
        {
            _travelService = travelService;
        }

        [HttpPost("recommendation")]
        public async Task<IActionResult> GetRecommendation([FromBody] TravelRequest request)
        {
            try
            {
                var recommendation = await _travelService.GetRecommendationAsync(request);
                return Ok(recommendation);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Result = "Error", Reason = ex.Message });
            }
        }
    }
}