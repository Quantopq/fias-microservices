
namespace ClientApiService.Interfaces
{
    public interface IClientService
    {
        Task<ClientResponseDto?> ProcessClientRequestAsync(ClientRequestDto request);
    }
}