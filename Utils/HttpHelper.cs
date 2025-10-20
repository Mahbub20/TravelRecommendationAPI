using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Security.Principal;
using System.Text.Json;
using System.Threading.Tasks;

namespace TravelRecommendationApi.Utils
{
    public class HttpHelper
    {
        private readonly HttpClient _client;
        public HttpHelper()
        {
            _client = new HttpClient { Timeout = TimeSpan.FromSeconds(60) };
        }

        public async Task<T?> GetAsync<T>(string url)
        {
            int retries = 3;
            for (int i = 0; i < retries; i++)
            {
                try
                {
                    var response = await _client.GetAsync(url);
                    response.EnsureSuccessStatusCode();
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }
                catch
                {
                    if (i == retries - 1) throw;   // last retry fail
                    await Task.Delay(200);          // wait before retry
                }
            }
            return default;
        }
    }
}