using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TravelRecommendationApi.Models
{
    public class District
    {
        public string Id { get; set; }
        public string DivisionId { get; set; }
        public string Name { get; set; }
        public string BnName { get; set; }
        public string Lat { get; set; }
        public string Long { get; set; }
    }
}