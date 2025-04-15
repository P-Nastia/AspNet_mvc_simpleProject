
using System.ComponentModel.DataAnnotations;

namespace Web1.Models.Category;

public class CategoryCreateViewModel
{
    [Display(Name = "Назва категорії")] // тобто в label буде виводитися це
    public string Name { get; set; } = string.Empty;

    [Display(Name = "Опис")]
    public string? Description { get; set; } = string.Empty;

    [Display(Name = "Оберіть фото")]
    
    //[DataType(DataType.ImageUrl)] // в input буде вказуватися type автоматично
    public IFormFile ImageFile { get; set; } = null!;
}
