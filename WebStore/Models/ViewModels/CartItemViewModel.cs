namespace WebStore.Models.ViewModels
{
    public class CartItemViewModel
    {
        public Product Product { get; set; }
        public int Quantity { get; set; }
        public double? Price { get; set; }
    }
}
