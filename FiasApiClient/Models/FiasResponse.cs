using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace FiasApiClient.Models
{
    public class FiasResponse
    {
        [JsonPropertyName("suggestions")]
        public List<FiasSuggestion> Suggestions { get; set; } = new();
    }
}