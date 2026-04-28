using Microsoft.AspNetCore.Mvc;
using FiasApiClient.Services;
using FiasApiClient.Models;
using System.Threading.Tasks;
using System;

namespace FiasApiClient.Controllers
{
    [ApiController]
    [Route("api/fias")]
    public class FiasController : ControllerBase
    {
        private readonly DadataFiasClient _fiasClient;
        private readonly ILogger<FiasController> _logger;

        public FiasController(DadataFiasClient fiasClient, ILogger<FiasController> logger)
        {
            _fiasClient = fiasClient;
            _logger = logger;
        }

        [HttpPost("suggest")]
        public async Task<IActionResult> GetKladr([FromBody] FiasRequestDto request)
        {
            try
            {
                _logger.LogInformation($"Запрос KLADR для адреса: {request.Region}");

                if (string.IsNullOrEmpty(request.Region))
                {
                    return BadRequest(new { error = "Region is required" });
                }

                var result = await _fiasClient.SearchAddressAsync(request.Region);

                if (result == null || result.Suggestions.Count == 0)
                {
                    return NotFound(new { client = request.Client, kladr = (string?)null });
                }

                var suggestion = result.Suggestions[0];
                var kladr = suggestion.Data.KladrId;

                _logger.LogInformation($"Найден KLADR: {kladr}");

                return Ok(new
                {
                    client = request.Client,
                    kladr = kladr
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при запросе к ФИАС");
                return StatusCode(500, new { error = "Внутренняя ошибка сервера" });
            }
        }
    }

    // DTO для запроса
    public class FiasRequestDto
    {
        public string Client { get; set; } = string.Empty;
        public string Region { get; set; } = string.Empty;
    }

}