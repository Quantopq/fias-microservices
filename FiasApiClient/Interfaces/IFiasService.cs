

namespace FiasApiClient.Services
{
	public interface IFiasService
	{
		Task<FiasResponse?> SearchAddressAsync(string address);
	}
}