using ClientApiService.Models;

namespace ClientApiService.Services
{
    public interface IClientService
    {
        Task<ClientResponseDto?> ProcessClientRequestAsync(ClientRequestDto request);
    }
}