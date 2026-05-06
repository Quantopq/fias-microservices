using Microsoft.AspNetCore.Mvc;
using ClientApiService.Models;
using ClientApiService.Services;

namespace ClientApiService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientController : ControllerBase
    {
        private readonly IClientService _clientService;
        private readonly ILogger<ClientController> _logger;

        public ClientController(
            IClientService clientService,
            ILogger<ClientController> logger)
        {
            _clientService = clientService;
            _logger = logger;
        }

        [HttpPost("process")]
        public async Task<IActionResult> ProcessClientRequest([FromBody] ClientRequestDto request)
        {
            try
            {
                var result = await _clientService.ProcessClientRequestAsync(request);

                if (result == null)
                {
                    return NotFound(new { error = "Адрес не найден в ФИАС" });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обработке запроса");
                return StatusCode(500, new { error = "Внутренняя ошибка сервера" });
            }
        }

        [HttpGet("health")]
        public IActionResult HealthCheck()
        {
            return Ok(new { status = "OK", timestamp = DateTime.UtcNow });
        }
    }
}