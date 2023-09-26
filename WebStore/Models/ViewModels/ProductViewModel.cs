namespace WebStore.Models.ViewModels
{
    public class ProductViewModel
    {
        public Product Product { get; set; }
        public string? Genre { get; set; }
        public string? SubGenre { get; set; }
        public double? Price { get; set; }
        public int? Quantity { get; set; }
    }
}
