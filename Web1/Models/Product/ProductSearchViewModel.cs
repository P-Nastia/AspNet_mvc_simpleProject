using System.ComponentModel.DataAnnotations;
using Web1.Models.Helpers;

namespace Web1.Models.Product;

public class ProductSearchViewModel
{
    [Display(Name ="Назва")]
    public string Name { get; set; } = string.Empty;
    [Display(Name = "Категорія")]
    public int CategoryId { get; set; }

    public List<SelectItemViewModel> Categories { get; set; } = new ();

    [Display(Name = "Опис")]
    public string Description { get; set; } = string.Empty;
}
