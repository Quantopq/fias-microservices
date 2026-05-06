using Microsoft.Extensions.Configuration;
using System.Text.Json;
using FiasApiClient.Models;
using System.Threading.Tasks;
using System;


namespace FiasApiClient.Services
{
    public class FiasService : IFiasService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly ILogger<FiasService> _logger;

        public FiasService(
            HttpClient httpClient,
            IConfiguration configuration,
            ILogger<FiasService> logger)
        {
            _httpClient = httpClient;
            _apiKey = configuration["DaApiKey"] ?? "";
            _logger = logger;

            _logger.LogInformation("API Key загружен: {KeyLength} символов", _apiKey.Length);
            _logger.LogInformation("API Key начинается с: {KeyStart}",
                _apiKey.Length > 10 ? _apiKey.Substring(0, 10) + "..." : "СЛИШКОМ КОРОТКИЙ");

            _httpClient.DefaultRequestHeaders.Remove("Authorization");
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Token {_apiKey}");
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<FiasResponse?> SearchAddressAsync(string address)
        {
            try
            {
                _logger.LogInformation("API Key: {Key}", _apiKey);

                var request = new { query = address };
                var json = JsonSerializer.Serialize(request);

                var content = new StringContent(
                    json,
                    System.Text.Encoding.UTF8,
                    "application/json");

                var fullUrl = "https://suggestions.dadata.ru/suggestions/api/4_1/rs/suggest/address";
                _logger.LogInformation("Запрос к: {Url}", fullUrl);

                var response = await _httpClient.PostAsync(fullUrl, content);

                _logger.LogInformation("Статус ответа: {StatusCode}", response.StatusCode);
                _logger.LogInformation("Content-Type ответа: {ContentType}",
                    response.Content.Headers.ContentType);

                var responseString = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    if (responseString.TrimStart().StartsWith("<"))
                    {
                        _logger.LogError("Dadata вернул HTML вместо JSON! Проверьте API Key!");
                        _logger.LogError("Ответ (первые 500 символов): {Response}",
                            responseString.Substring(0, Math.Min(500, responseString.Length)));
                        return null;
                    }

                    return JsonSerializer.Deserialize<FiasResponse>(responseString);
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Исключение в методе SearchAddressAsync");
                return null;
            }
        }
    }
}