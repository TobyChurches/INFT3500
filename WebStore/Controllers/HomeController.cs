using Microsoft.AspNetCore.Mvc;
using WebStore.Models;
using WebStore.Models.ViewModels;

namespace WebStore.Controllers;

public class HomeController : Controller
{
    private readonly StoreDbContext _context;

    public HomeController(StoreDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        // Get username from session
        var customer = 11;

        var orders = _context.Orders
            .Where(x => x.Customer == 11)
            .Select(x => new OrderViewModel()
            {
                Name = "",
                Address = x.StreetAddress,
                OrderId = x.OrderId
            })
            .ToList();

        if (orders.Count == 0)
        {
            ViewData["OrderMessage"] = "You do not have any orders associated with your account.";
        }

        return View(orders);
    }

    public IActionResult OrderDetails()
    {
        return View();
    }

    public IActionResult Account()
    {
        return View();
    }
}