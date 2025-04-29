using System.ComponentModel.DataAnnotations;

namespace Web1.Models.Account;

public class LoginViewModel
{
    [Display(Name = "Електронна пошта")]
    [Required(ErrorMessage ="Вкажіть електронну пошту")]
    [EmailAddress(ErrorMessage ="Пошту вказано не правильно")]
    public string Email { get; set; } = String.Empty;

    [Display(Name = "Пароль")]
    [Required(ErrorMessage = "Вкажіть пароль")]
    public string Password { get; set; } = String.Empty;
}
