namespace Blazor6.Shared.Models;

public class ProductOrderRowDto : ProductDto
{
    public int Quantity { get; set; }

    public double TotalPrice { get; set; }

    public ProductOrderRowDto(ProductDto product)
    {
        Id = product.Id;
        Description = product.Description;
        Name = product.Name;
        ImageUrl = product.ImageUrl;
        Stock = product.Stock;
        Price = product.Price;
    }
}