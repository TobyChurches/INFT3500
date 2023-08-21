using Microsoft.AspNetCore.Mvc;

namespace WebStore.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult EditItem()
        {
            return View();
        }

        public IActionResult ItemList()
        {
            return View();
        }

        public IActionResult EditUser()
        {
            return View();
        }

        public IActionResult UserList()
        {
            return View();
        }
    }
}
