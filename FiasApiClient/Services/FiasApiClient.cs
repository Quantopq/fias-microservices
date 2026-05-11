using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using FiasApiClient.Models;

namespace FiasApiClient.Services
{
	public class DadataFiasClient  
	{
		private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private const string BaseUrl = "https://suggestions.dadata.ru/suggestions/api/4_1/rs/suggest/fias";

        public DadataFiasClient(string apiKey)
        {
            _apiKey = apiKey;
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(BaseUrl),
                Timeout = TimeSpan.FromSeconds(30)
            };
            
            // Настройка заголовков
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue("Token", apiKey);
        }

        public async Task<FiasResponse?> SearchAddressAsync(string query)
        {
            var request = new FiasRequest { Query = query, Count = 5 };
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync("", content);
                response.EnsureSuccessStatusCode();
                
                var responseString = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<FiasResponse>(responseString);
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"❌ Ошибка запроса: {ex.Message}");
                return null;
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"❌ Ошибка парсинга JSON: {ex.Message}");
                return null;
            }
        }


        public void Dispose() => _httpClient?.Dispose();
    }
}