using System.Text.Json.Serialization;

namespace FiasApiClient.Models
{
    public class FiasSuggestion
    {
        [JsonPropertyName("value")]  
        public string Value { get; set; } = string.Empty;

        [JsonPropertyName("data")]  
        public FiasData Data { get; set; } = new();
    }
}