using Microsoft.AspNetCore.Mvc;
using ClientApiService.Models;
using ClientApiService.Data;
using System.Text;
using System.Text.Json;

namespace ClientApiService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<ClientController> _logger;
        private readonly string _fiasServiceUrl;

        public ClientController(
            AppDbContext context, 
            IHttpClientFactory httpClientFactory,
            ILogger<ClientController> logger,
            IConfiguration configuration)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _fiasServiceUrl = configuration["FiasServiceUrl"] 
                ?? "http://fias-api:8080";
        }

        [HttpPost("process")]
        public async Task<IActionResult> ProcessClientRequest([FromBody] ClientRequestDto request)
        {
            try
            {
                _logger.LogInformation($"ќбработка запроса от клиента {request.Client} дл€ адреса: {request.Region}");

                var fiasResponse = await SendRequestToFiasService(request);

                if (fiasResponse == null || string.IsNullOrEmpty(fiasResponse.kladr)) 
                {
                    _logger.LogWarning($"KLADR не найден дл€ адреса: {request.Region}");
                    return NotFound(new { error = "јдрес не найден в ‘»ј—" });
                }

                var result = new RequestResult
                {
                    Client = request.Client,
                    Kladr = fiasResponse.kladr,  
                    ResponseDate = DateTime.UtcNow
                };

                _context.RequestResults.Add(result);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"—охранен результат: Client={request.Client}, KLADR={fiasResponse.kladr}");  

                return Ok(new ClientResponseDto
                {
                    client = request.Client, 
                    kladr = fiasResponse.kladr  
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ќшибка при обработке запроса");
                return StatusCode(500, new { error = "¬нутренн€€ ошибка сервера" });
            }
        }

        private async Task<ClientResponseDto?> SendRequestToFiasService(ClientRequestDto request)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                client.Timeout = TimeSpan.FromSeconds(30);

                var content = new StringContent(
                    JsonSerializer.Serialize(request),
                    Encoding.UTF8,
                    "application/json");

                var response = await client.PostAsync($"{_fiasServiceUrl}/api/Fias/suggest", content);

                if (response.IsSuccessStatusCode)
                {

                    var responseString = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<ClientResponseDto>(responseString);
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ќшибка при запросе к FIAS сервису");
                return null;
            }
        }

        // Ёндпоинт дл€ проверки статуса
        [HttpGet("health")]
        public IActionResult HealthCheck()
        {
            return Ok(new { status = "OK", timestamp = DateTime.UtcNow });
        }
    }
}