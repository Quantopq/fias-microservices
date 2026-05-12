
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
            _fiasServiceUrl = configuration["FiasService:BaseUrl"] ?? "http://fias-api:8080";
        }

        public async Task<ClientResponseDto?> ProcessClientRequestAsync(ClientRequestDto request)
        {
            try
            {
                _logger.LogInformation("Обработка запроса от клиента {Client} для адреса: {Region}",
                    request.Client, request.Region);

                var fiasResponse = await SendRequestToFiasServiceAsync(request);

                if (fiasResponse == null || string.IsNullOrEmpty(fiasResponse.kladr))
                {
                    _logger.LogWarning("KLADR не найден для адреса: {Region}", request.Region);
                    return null;
                }

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


        public async Task<IEnumerable<LeadResponseDto>> GetAllLeadsAsync()
        {
            var leads = await _context.Leads
                .OrderByDescending(l => l.CreatedAt)
                .ToListAsync();

            return leads.Select(MapToResponseDto);
        }

        public async Task<IEnumerable<LeadResponseDto>> GetPendingLeadsAsync()
        {
            var leads = await _context.Leads
                .Where(l => l.Status == "Pending")
                .OrderBy(l => l.CreatedAt)
                .ToListAsync();

            return leads.Select(MapToResponseDto);
        }

        public async Task<LeadResponseDto?> CreateLeadAsync(LeadRequestDto request)
        {
            var lead = new Lead
            {
                ClientId = request.ClientId,
                ClientName = request.ClientName,
                RawAddress = request.RawAddress,
                Status = "Pending"
            };

            _context.Leads.Add(lead);
            await _context.SaveChangesAsync();

            return MapToResponseDto(lead);
        }

        public async Task<LeadResponseDto?> ProcessLeadAsync(int leadId)
        {
            var lead = await _context.Leads.FindAsync(leadId);
            if (lead == null || lead.Status != "Pending")
                return null;

            try
            {
                // Вызываем FIAS API для получения исправленного адреса
                var fiasRequest = new ClientRequestDto
                {
                    Client = lead.ClientId,
                    Region = lead.RawAddress
                };

                var fiasResponse = await SendRequestToFiasServiceAsync(fiasRequest);

                if (fiasResponse != null && !string.IsNullOrEmpty(fiasResponse.kladr))
                {
                    // Обновляем лид
                    lead.CorrectedAddress = fiasResponse.client; // или распарсить адрес из ответа
                    lead.Kladr = fiasResponse.kladr;
                    lead.Status = "Processed";
                    lead.ProcessedAt = DateTime.UtcNow;

                    _context.Leads.Update(lead);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    lead.Status = "Error";
                    _context.Leads.Update(lead);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обработке лида {LeadId}", leadId);
                lead.Status = "Error";
                _context.Leads.Update(lead);
                await _context.SaveChangesAsync();
            }

            return MapToResponseDto(lead);
        }

        // Вспомогательный метод маппинга:
        private LeadResponseDto MapToResponseDto(Lead lead) => new()
        {
            Id = lead.Id,
            ClientId = lead.ClientId,
            ClientName = lead.ClientName,
            RawAddress = lead.RawAddress,
            CorrectedAddress = lead.CorrectedAddress,
            Kladr = lead.Kladr,
            Status = lead.Status,
            CreatedAt = lead.CreatedAt,
            ProcessedAt = lead.ProcessedAt
        };
    }
}