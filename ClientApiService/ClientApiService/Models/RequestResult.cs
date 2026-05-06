using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClientApiService.Models
{
    [Table("requestResult")]
    public class RequestResult
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Client { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Kladr { get; set; } = string.Empty;

        [Required]
        public DateTime ResponseDate { get; set; } = DateTime.UtcNow;
    }
}