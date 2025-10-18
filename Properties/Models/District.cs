using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TravelRecommendationApi.Properties.Models
{
    public class District
    {
        public int Id { get; set; }
        public int DivisionId { get; set; }
        public string Name { get; set; }
        public string BnName { get; set; }
        public double Lat { get; set; }
        public double Long { get; set; }
    }
}