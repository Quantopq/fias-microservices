namespace ClientApiService.Models
{
    public class LeadResponseDto
    {
        public int Id { get; set; }
        public string ClientId { get; set; } = string.Empty;
        public string? ClientName { get; set; }
        public string RawAddress { get; set; } = string.Empty;
        public string? CorrectedAddress { get; set; }
        public string? Kladr { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? ProcessedAt { get; set; }
    }
}