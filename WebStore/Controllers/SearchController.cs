using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using WebStore.Models;
using WebStore.Models.ViewModels;

namespace WebStore.Controllers;

[Authorize(Roles = "Customer")]
public class SearchController : Controller
{
    private readonly StoreDbContext _context;

    public SearchController(StoreDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult ItemDetails(int id)
    {
        var vm = _context.Products
            .Where(x => x.Id == id)
            .Select(p => new ProductViewModel()
            {
                Product = p,
                Genre = p.GenreNavigation.Name,
                Price = p.Stocktakes
                    .Select(s => s.Price)
                    .FirstOrDefault(),
                Quantity = p.Stocktakes
                    .Select(s => s.Quantity)
                    .FirstOrDefault()
            })
            .FirstOrDefault();

        if (vm.Product == null)
        {
            throw new Exception("Product not found");
        }

        vm.SubGenre = getSubGenre(vm.Genre, vm.Product.SubGenre);

        return View(vm);
    }

    [HttpGet]
    public IActionResult SearchResults()
    {
        var searchQuery = HttpContext.Session.GetString("SearchQuery") ?? "";
        return SearchResults(searchQuery);
    }

    [HttpPost]
    public ActionResult SearchResults(string searchQuery)
    {
        searchQuery = searchQuery.IsNullOrEmpty() ? "" : searchQuery.ToLower().Trim();
        var results = _context.Products
            .Where(p => p.Name.ToLower().Contains(searchQuery)
                        || p.Author.ToLower().Contains(searchQuery)
                        || p.Description.ToLower().Contains(searchQuery))
            .Select(p => new ProductViewModel()
            {
                Product = p,
                Genre = p.GenreNavigation.Name,
                Price = p.Stocktakes
                    .Select(s => s.Price)
                    .FirstOrDefault()
            })
            .ToList();

        foreach (var result in results)
        {
            result.SubGenre = getSubGenre(result.Genre, result.Product.SubGenre);
        }

        HttpContext.Session.SetString("SearchQuery", searchQuery);
        return View(results);
    }

    private string? getSubGenre(string? genre, int? subGenreId)
    {
        if (subGenreId == null || genre == null)
        {
            return null;
        }

        if(genre.Contains("Book", StringComparison.OrdinalIgnoreCase))
        {
            return GetSubGenres(_context.BookGenres, subGenreId);
        }
        else if (genre.Contains("Movie", StringComparison.OrdinalIgnoreCase))
        {
            return GetSubGenres(_context.MovieGenres, subGenreId);
        }
        else if (genre.Contains("Game", StringComparison.OrdinalIgnoreCase))
        {
            return GetSubGenres(_context.GameGenres, subGenreId);
        }
        else
        {
            return null;
        }
    }

    private string? GetSubGenres(IEnumerable<dynamic> subGenreCollection, int? id)
    {
        return subGenreCollection
            .Where(x => x.SubGenreId == id)
            .Select(x => x.Name)
            .FirstOrDefault();
    }
}
