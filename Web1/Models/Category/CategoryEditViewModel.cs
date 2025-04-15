
using System.ComponentModel.DataAnnotations;

namespace Web1.Models.Category;

public class CategoryEditViewModel
{
    public int Id { get; set; }

    [Display(Name = "Назва категорії")] // тобто в label буде виводитися це
    public string Name { get; set; } = string.Empty;

    [Display(Name = "Опис")]
    public string? Description { get; set; } = string.Empty;

    [Display(Name = "Url адреса фото")]
    //[DataType(DataType.ImageUrl)] // в input буде вказуватися type автоматично
    public string ImageUrl { get; set; } = string.Empty;
}
