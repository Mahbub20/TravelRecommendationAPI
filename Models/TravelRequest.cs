using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TravelRecommendationApi.Models
{
    public class TravelRequest
    {
        public double CurrentLat { get; set; }
        public double CurrentLong { get; set; }
        public string DestinationDistrict { get; set; }
        public DateTime TravelDate { get; set; }
    }
}