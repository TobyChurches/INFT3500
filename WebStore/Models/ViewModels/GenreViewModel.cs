namespace WebStore.Models.ViewModels
{
    public class GenreViewModel
    {
        public Genre Genre { get; set; }
        public List<Dictionary<int, string>> SubGenre { get; set; }
    }
}
