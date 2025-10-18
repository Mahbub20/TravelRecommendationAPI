using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text.Json;
using System.Threading.Tasks;

namespace TravelRecommendationApi.Utils
{
    public class HttpHelper
    {
        private readonly HttpClient _client;
        public HttpHelper()
        {
            _client = new HttpClient { Timeout = TimeSpan.FromSeconds(10) };
        }

        public async Task<T?> GetAsync<T>(string url)
        {
            var response = await _client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
    }
}