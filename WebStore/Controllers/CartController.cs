using Microsoft.AspNetCore.Mvc;

namespace WebStore.Controllers;

public class CartController : Controller
{
    public IActionResult Cart()
    {
        return View();
    }
}
