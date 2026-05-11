using System.Text.Json.Serialization;

namespace FiasApiClient.Models
{
    public class FiasSuggestion
    {
        [JsonPropertyName("value")]  // ← Добавь!
        public string Value { get; set; } = string.Empty;

        [JsonPropertyName("data")]  // ← Добавь!
        public FiasData Data { get; set; } = new();
    }
}