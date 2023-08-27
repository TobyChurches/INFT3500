using Microsoft.AspNetCore.Mvc;
using WebStore.Models;

namespace WebStore.Controllers;

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
        var product = _context.Products.Find(id);
        return View(product);
    }

    [HttpPost]
    public ActionResult SearchResults(string searchQuery)
    {
        searchQuery = searchQuery.ToLower();
        var results = _context.Products
            .Where(p => p.Name.ToLower().Contains(searchQuery)
                        || p.Author.ToLower().Contains(searchQuery)
                        || p.Description.ToLower().Contains(searchQuery))
            .ToList();

        return View(results);
    }
}
