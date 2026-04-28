using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace FiasApiClient.Models
{
    public class FiasSuggestion
    {
        [JsonPropertyName("value")]
        public string Value { get; set; } = string.Empty;
        
        [JsonPropertyName("unrestricted_value")]
        public string UnrestrictedValue { get; set; } = string.Empty;
        
        [JsonPropertyName("data")]
        public FiasData Data { get; set; } = new();
    }

    public class FiasData
    {
        [JsonPropertyName("kladr_id")]
        public string? KladrId { get; set; }
        
        [JsonPropertyName("fias_id")]
        public string? FiasId { get; set; }
        
        [JsonPropertyName("region_with_type")]
        public string? RegionWithType { get; set; }
        
        [JsonPropertyName("city_with_type")]
        public string? CityWithType { get; set; }
        
        [JsonPropertyName("street_with_type")]
        public string? StreetWithType { get; set; }
        
        [JsonPropertyName("house")]
        public string? House { get; set; }
        
        [JsonPropertyName("postal_code")]
        public string? PostalCode { get; set; }
    }
}