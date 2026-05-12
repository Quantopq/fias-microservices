
namespace ClientApiService.Models
{
    [Table("Leads")]
    public class Lead
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string ClientId { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? ClientName { get; set; }

        [Required]
        [MaxLength(500)]
        public string RawAddress { get; set; } = string.Empty;  // "кривой" адрес

        [MaxLength(500)]
        public string? CorrectedAddress { get; set; }  // исправленный адрес

        [MaxLength(50)]
        public string? Kladr { get; set; }

        [MaxLength(50)]
        public string Status { get; set; } = "Pending";  // Pending, Processed, Error

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ProcessedAt { get; set; }
    }
}