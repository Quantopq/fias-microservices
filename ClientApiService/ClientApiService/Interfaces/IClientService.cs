
namespace ClientApiService.Interfaces
{
    public interface IClientService
    {
        Task<ClientResponseDto?> ProcessClientRequestAsync(ClientRequestDto request);
        Task<IEnumerable<LeadResponseDto>> GetAllLeadsAsync();
        Task<IEnumerable<LeadResponseDto>> GetPendingLeadsAsync();
        Task<LeadResponseDto?> CreateLeadAsync(LeadRequestDto request);
        Task<LeadResponseDto?> ProcessLeadAsync(int leadId);
    }
}