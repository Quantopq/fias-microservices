
namespace ClientApiService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LeadController : ControllerBase
    {
        private readonly IClientService _clientService;
        private readonly ILogger<LeadController> _logger;

        public LeadController(IClientService clientService, ILogger<LeadController> logger)
        {
            _clientService = clientService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllLeads()
        {
            var leads = await _clientService.GetAllLeadsAsync();
            return Ok(leads);
        }

        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingLeads()
        {
            var leads = await _clientService.GetPendingLeadsAsync();
            return Ok(leads);
        }

        [HttpPost]
        public async Task<IActionResult> CreateLead([FromBody] LeadRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var lead = await _clientService.CreateLeadAsync(request);
            return CreatedAtAction(nameof(GetLeadById), new { id = lead?.Id }, lead);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetLeadById(int id)
        {
            var leads = await _clientService.GetAllLeadsAsync();
            var lead = leads.FirstOrDefault(l => l.Id == id);
            
            return lead != null ? Ok(lead) : NotFound();
        }

        [HttpPost("{id}/process")]
        public async Task<IActionResult> ProcessLead(int id)
        {
            var result = await _clientService.ProcessLeadAsync(id);
            
            return result != null 
                ? Ok(result) 
                : NotFound(new { error = "Lead not found or already processed" });
        }
    }
}