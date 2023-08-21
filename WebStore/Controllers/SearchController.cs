using Microsoft.AspNetCore.Mvc;

namespace WebStore.Controllers
{
    public class SearchController : Controller
    {
        public IActionResult ItemDetails()
        {
            return View();
        }

        public IActionResult SearchResults()
        {
            return View();
        }
    }
}
