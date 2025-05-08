using System.ComponentModel.DataAnnotations;

namespace Web1.Models.Account
{
    public class ForgotPasswordViewModel
    {
        [Display(Name = "Ваша пошта")]
        [Required(ErrorMessage = "Вкажіть електронну пошту")]
        [EmailAddress(ErrorMessage = "Пошту вказано неправильно")]

        public string Email { get; set; } = string.Empty;
    }
}
