namespace WebStore.Models.ViewModels
{
    public class AdminProductViewModel
    {
        public Product Product { get; set; }
        public List<GenreViewModel>? Genres { get; set; }
        public Stocktake Stocktake { get; set; }
        public List<Source>? Sources { get; set; }
        public Source Source { get; set; }
    }
}
