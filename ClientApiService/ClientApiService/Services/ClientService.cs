using ClientApiService.Data;
using ClientApiService.Models;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.Json;

namespace ClientApiService.Services
{
    public class ClientService : IClientService
    {
        private readonly AppDbContext _context;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<ClientService> _logger;
        private readonly string _fiasServiceUrl;

        public ClientService(
            AppDbContext context,
            IHttpClientFactory httpClientFactory,
            ILogger<ClientService> logger,
            IConfiguration configuration)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _fiasServiceUrl = configuration["FiasServiceUrl"] ?? "http://fias-api:8080";
        }

        public async Task<ClientResponseDto?> ProcessClientRequestAsync(ClientRequestDto request)
        {
            try
            {
                _logger.LogInformation("Обработка запроса от клиента {Client} для адреса: {Region}", 
                    request.Client, request.Region);

                // Отправляем запрос в FIAS сервис
                var fiasResponse = await SendRequestToFiasServiceAsync(request);
                
                if (fiasResponse == null || string.IsNullOrEmpty(fiasResponse.kladr))
                {
                    _logger.LogWarning("KLADR не найден для адреса: {Region}", request.Region);
                    return null;
                }

                // Сохраняем результат в БД
                var result = new RequestResult
                {
                    Client = request.Client,
                    Kladr = fiasResponse.kladr,
                    ResponseDate = DateTime.UtcNow
                };

                _context.RequestResults.Add(result);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Сохранен результат: Client={Client}, KLADR={Kladr}", 
                    request.Client, fiasResponse.kladr);

                return new ClientResponseDto
                {
                    client = request.Client,
                    kladr = fiasResponse.kladr
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обработке запроса");
                throw;
            }
        }

        private async Task<ClientResponseDto?> SendRequestToFiasServiceAsync(ClientRequestDto request)
        {
            var client = _httpClientFactory.CreateClient();
            
            var content = new StringContent(
                JsonSerializer.Serialize(request),
                Encoding.UTF8,
                "application/json");

            var response = await client.PostAsync($"{_fiasServiceUrl}/api/fias/suggest", content);
            
            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<ClientResponseDto>(responseString);
            }
                
            return null;
        }
    }
}