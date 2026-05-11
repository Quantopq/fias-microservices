using System.Text.Json.Serialization;  

namespace FiasApiClient.Models
{
    public class FiasResponse
    {
        [JsonPropertyName("suggestions")]
        public List<FiasSuggestion> Suggestions { get; set; } = new();
    }
}