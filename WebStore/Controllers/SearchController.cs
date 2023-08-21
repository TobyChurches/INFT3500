using Microsoft.AspNetCore.Mvc;
using WebStore.Models;

namespace WebStore.Controllers
{
    public class SearchController : Controller
    {
        private readonly StoreDbContext _context;

        public SearchController(StoreDbContext context)
        {
            _context = context;
        }

        public IActionResult ItemDetails()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SearchResults(string searchQuery)
        {
            // Handle the search logic here

            ViewBag.SearchQuery = searchQuery;
            return View();
        }
    }
}
