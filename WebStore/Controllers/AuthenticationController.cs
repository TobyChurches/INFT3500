using Microsoft.AspNetCore.Mvc;

namespace WebStore.Controllers
{
    public class AuthenticationController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }

        public IActionResult CreateAccount()
        {
            return View();
        }
    }
}
