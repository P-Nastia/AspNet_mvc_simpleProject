using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Web1.Models.Product;

public class ProductListViewModel
{
    // відображення писку продуктів
    public List<ProductItemViewModel> Products { get; set; } = new();
    // модель для пошуку
    public ProductSearchViewModel Search { get; set; } = new();

    // к-ість елементів під час пошуку
    public int Count { get; set; }
}
