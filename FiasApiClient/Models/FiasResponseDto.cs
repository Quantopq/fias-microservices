using System.ComponentModel.DataAnnotations;

namespace FiasApiClient.Models
{
    public class FiasRequestDto
    {
        [Required(ErrorMessage = "Client is required")]
        [StringLength(100, ErrorMessage = "Client cannot be longer than 100 characters")]
        public string Client { get; set; } = string.Empty;

        [Required(ErrorMessage = "Region is required")]
        [StringLength(500, ErrorMessage = "Region cannot be longer than 500 characters")]
        [MinLength(1, ErrorMessage = "Region cannot be empty")]
        public string Region { get; set; } = string.Empty;
    }
}