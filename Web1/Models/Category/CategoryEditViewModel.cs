
using System.ComponentModel.DataAnnotations;

namespace Web1.Models.Category;

public class CategoryEditViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage ="Назва де?")]
    [Display(Name = "Назва категорії")] // тобто в label буде виводитися це
    public string Name { get; set; } = string.Empty;

    [Display(Name = "Опис")]
    public string? Description { get; set; } = string.Empty;

    public string? ViewImage { get; set; } = string.Empty;

    [Display(Name = "Оберіть фото")]
    //[DataType(DataType.ImageUrl)] // в input буде вказуватися type автоматично
    public IFormFile ? ImageFile { get; set; } = null!; // ? -- означає, що поле не буде перевірятися на валідність, бо користувач може не обирати нове фото
}
