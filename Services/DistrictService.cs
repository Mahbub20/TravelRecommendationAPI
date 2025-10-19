using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using TravelRecommendationApi.Models;

namespace TravelRecommendationApi.Services
{
    public class DistrictService
    {
        private List<District> _districts = new();

        public DistrictService()
        {
            LoadDistricts();
        }

        private void LoadDistricts()
        {
            var json = File.ReadAllText("data/bd-districts.json");
            var wrapper = JsonSerializer.Deserialize<JsonElement>(json);
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            _districts = JsonSerializer.Deserialize<List<District>>(wrapper.GetProperty("districts"), options)!;
        }

        public List<District> GetAllDistricts() => _districts;

        public District? GetDistrictByName(string name) =>
            _districts.FirstOrDefault(d => d.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
    }
}