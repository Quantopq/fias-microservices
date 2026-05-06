namespace ClientApiService.Models
{
    public class ClientRequestDto
    {
        public string Client { get; set; } = string.Empty;
        public string Region { get; set; } = string.Empty;
    }

    public class ClientResponseDto
    {
        public string client { get; set; } = string.Empty;
        public string kladr { get; set; } = string.Empty;
    }
}