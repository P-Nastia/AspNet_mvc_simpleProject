using System.ComponentModel.DataAnnotations;

namespace Web1.Models.Account
{
    public class ForgotPasswordViewModel
    {
        [Display(Name = "Ваша пошта")]
        [Required(ErrorMessage = "Вкажіть електронну пошту")]
        [EmailAddress(ErrorMessage = "Пошту вказано неправильно")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } = string.Empty;
    }
}
