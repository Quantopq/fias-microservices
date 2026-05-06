using System.ComponentModel.DataAnnotations;

namespace FiasApiClient.Models
{
    public class FiasRequestDto
    {
        [Required(ErrorMessage = "Поле 'Client' обязательно для заполнения")]
        [StringLength(100, ErrorMessage = "Длина поля 'Client' не должна превышать 100 символов")]
        public string Client { get; set; } = string.Empty;

        [Required(ErrorMessage = "Поле 'Region' обязательно для заполнения")]
        [StringLength(500, ErrorMessage = "Длина поля 'Region' не должна превышать 500 символов")]
        [MinLength(1, ErrorMessage = "Поле 'Region' не может быть пустым")]
        public string Region { get; set; } = string.Empty;
    }
}