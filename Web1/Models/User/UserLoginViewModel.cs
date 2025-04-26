using System.ComponentModel.DataAnnotations;

namespace Web1.Models.User;

public class UserLoginViewModel
{

    [Display(Name = "Ваш псевдонім")]
    [DataType(DataType.Text)]
    [Required(ErrorMessage = "Нікнейм обов'язковий")]
    public string Nickname { get; set; } = String.Empty;


    [Display(Name = "Пароль до акаунту")]
    [DataType(DataType.Password)]
    [Required(ErrorMessage = "Пароль обов'язковий")]
    public string Password { get; set; } = String.Empty;
}
