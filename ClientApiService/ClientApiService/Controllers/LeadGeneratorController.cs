using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ClientApiService.Services;

namespace ClientApiService.Controllers;

[ApiController]
[Route("api/[controller]")]
//[Authorize(Roles = "Admin,Operator")]
public class LeadGeneratorController : ControllerBase
{
    private readonly ILeadGeneratorService _generator;
    private readonly ILogger<LeadGeneratorController> _logger;

    public LeadGeneratorController(
        ILeadGeneratorService generator,
        ILogger<LeadGeneratorController> logger)
    {
        _generator = generator;
        _logger = logger;
    }

    [HttpPost("generate")]
    public async Task<IActionResult> GenerateLead()
    {
        try
        {
            var lead = await _generator.GenerateRandomLeadAsync();
            _logger.LogInformation("Manually generated lead: {Name}, {Address}", 
                lead.ClientName, lead.BadAddress);
            
            return Ok(lead);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating lead");
            return StatusCode(500, "Error generating lead");
        }
    }
}