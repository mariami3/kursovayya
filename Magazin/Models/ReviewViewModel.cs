using System.ComponentModel.DataAnnotations;

namespace Magazin.Models
{
    public class ReviewViewModel
    {
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        [Range(1, 5, ErrorMessage = "Оценка должна быть от 1 до 5")]
        public int Rating { get; set; }

        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        [StringLength(1000, MinimumLength = 1, ErrorMessage = "Комментарий должен содержать от 1 до 1000 символов")]
        public string Comment { get; set; } = string.Empty;

        [Required]
        public int BookId { get; set; }
    }
}