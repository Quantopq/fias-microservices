namespace ClientApiService.Models
{
    public class LeadRequestDto
    {
        public string ClientId { get; set; } = string.Empty;
        public string? ClientName { get; set; }
        public string RawAddress { get; set; } = string.Empty;
    }
}