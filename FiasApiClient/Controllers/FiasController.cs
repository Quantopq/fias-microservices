using Microsoft.AspNetCore.Mvc;
using FiasApiClient.Services;
using FiasApiClient.Models;

namespace FiasApiClient.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FiasController : ControllerBase
    {
        private readonly IFiasService _fiasService;
        private readonly ILogger<FiasController> _logger;

        public FiasController(IFiasService fiasService, ILogger<FiasController> logger)
        {
            _fiasService = fiasService;
            _logger = logger;
        }

        [HttpPost("suggest")]
        public async Task<IActionResult> GetKladr([FromBody] FiasRequestDto request)
        {
            _logger.LogInformation("Запрос KLADR для адреса: {Address}", request.Region);

            if (string.IsNullOrEmpty(request.Region))
            {
                return BadRequest(new { error = "Region is required" });
            }

            var result = await _fiasService.SearchAddressAsync(request.Region);

            // Исправленная проверка null
            if (result == null || result.Suggestions == null || result.Suggestions.Count == 0)
            {
                return NotFound(new { client = request.Client, kladr = (string?)null });
            }

            var kladr = result.Suggestions[0].Data.KladrId;
            _logger.LogInformation("Найден KLADR: {Kladr}", kladr);

            return Ok(new { client = request.Client, kladr });
        }
    }
}