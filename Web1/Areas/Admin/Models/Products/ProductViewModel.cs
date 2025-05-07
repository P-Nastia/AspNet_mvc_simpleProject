namespace Web1.Areas.Admin.Models.Products;

public class ProductItemViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public float Price { get; set; } 
    public List<string> Images { get; set; } = new();
}
