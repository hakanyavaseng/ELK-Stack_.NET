using ELK.WebAPI.Entities.Common;

namespace ELK.WebAPI.Entities;

public class Product : IElasticsearchModal
{
    public string Name { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}