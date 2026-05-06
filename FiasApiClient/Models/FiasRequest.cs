using System.Text.Json.Serialization;

namespace FiasApiClient.Models
{
    public class FiasRequest
    {
        [JsonPropertyName("query")]
        public string Query { get; set; } = string.Empty;
        
        [JsonPropertyName("count")]
        public int Count { get; set; } = 10; // Макс. 20
    }
}