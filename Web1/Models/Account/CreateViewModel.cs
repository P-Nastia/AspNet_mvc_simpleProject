using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Web1.Models.Account
{
    public class CreateViewModel
    {
        [Display(Name = "Ваше ім'я")]
        [DataType(DataType.Text)]
        [Required(ErrorMessage = "Ім'я обов'язкове")]
        public string FirstName { get; set; } = String.Empty;

        [Display(Name = "Ваш псевдонім")]
        [DataType(DataType.Text)]
        [Required(ErrorMessage = "Нікнейм обов'язковий")]
        public string UserName { get; set; } = String.Empty;

        [Display(Name = "Ваше прізвище")]
        [DataType(DataType.Text)]
        [Required(ErrorMessage = "Прізвище обов'язкове")]
        public string LastName { get; set; } = String.Empty;

        [Display(Name = "Ваша пошта")]
        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "Вкажіть електронну пошту")]
        [EmailAddress(ErrorMessage = "Пошту вказано не правильно")]
        public string Email { get; set; } = String.Empty;

        [Display(Name = "Пароль до акаунту")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Пароль обов'язковий")]
        public string Password { get; set; } = String.Empty;

        [Display(Name = "Ваш номер телефону")]
        [DataType(DataType.PhoneNumber)]
        [Required(ErrorMessage = "Номер телефону обов'язковий")]
        [Phone(ErrorMessage ="Номер вказано не правильно")]
        public string PhoneNumber { get; set; } = String.Empty;

        [Display(Name = "Оберіть заставку")]
        [Required(ErrorMessage = "Фото обов'язкове")]
        public IFormFile? ImageFile { get; set; } = null;
    }
}
