using System.Text.Json.Serialization;

namespace FiasApiClient.Models
{
    public class FiasData
    {
        [JsonPropertyName("kladr_id")]  // ← Добавь! underscore важен!
        public string KladrId { get; set; } = string.Empty;
    }
}