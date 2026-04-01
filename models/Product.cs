namespace PawCareApi.models
{
   

    public class Product
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;  
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public int Stock { get; set; }
    }
}
