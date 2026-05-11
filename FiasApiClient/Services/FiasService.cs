
namespace FiasApiClient.Services
{
    public class FiasService : IFiasService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly ILogger<FiasService> _logger;
        private readonly string _baseUrl;

        public FiasService(
            HttpClient httpClient,
            IConfiguration configuration,
            ILogger<FiasService> logger)
        {
            _httpClient = httpClient;
            _apiKey = configuration["Dadata:ApiKey"] ?? "";
            _baseUrl = configuration["Dadata:BaseUrl"] ?? "https://suggestions.dadata.ru/suggestions/api/4_1/rs";
            _logger = logger;

            _logger.LogInformation("API Key загружен: {KeyLength} символов", _apiKey.Length);
            _logger.LogInformation("Base URL: {BaseUrl}", _baseUrl);

            _httpClient.DefaultRequestHeaders.Remove("Authorization");
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Token {_apiKey}");
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<FiasResponse?> SearchAddressAsync(string address)
        {
            try
            {
                _logger.LogInformation("Запрос к Dadata для адреса: {Address}", address);

                var request = new { query = address };
                var json = JsonSerializer.Serialize(request);

                var content = new StringContent(
                    json,
                    Encoding.UTF8,
                    "application/json");

                _logger.LogInformation("Запрос к: {Url}", $"{_baseUrl}/suggest/address");

                var response = await _httpClient.PostAsync($"{_baseUrl}/suggest/address", content);

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
                            responseString.Length > 500 ? responseString.Substring(0, 500) : responseString);
                        return null;
                    }

                    var result = JsonSerializer.Deserialize<FiasResponse>(responseString);
                    _logger.LogInformation("Успешная десериализация ответа");
                    _logger.LogInformation("Количество suggestions: {Count}", result.Suggestions.Count);

                    if (result.Suggestions.Count > 0)
                    {
                        _logger.LogInformation("Первый KLADR: {Kladr}", result.Suggestions[0].Data.KladrId);
                    }
                    else
                    {
                        _logger.LogWarning("Dadata не нашла адрес! Полный ответ: {Response}", responseString);
                    }
                    return result;
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