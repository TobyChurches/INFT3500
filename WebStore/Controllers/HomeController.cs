using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebStore.Models;
using WebStore.Models.ViewModels;

namespace WebStore.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly StoreDbContext _context;
    private readonly UserManager<User> _userManager;

    public HomeController(StoreDbContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var user = await _userManager.FindByNameAsync(User.Identity?.Name);
        var customerIds = _context.Tos
            .Where(x => x.PatronId == user.UserId)
            .Select(x => x.CustomerId)
            .ToList();

        var orders = _context.Orders
            .Where(x => x.Customer != null && customerIds.Contains(x.Customer.Value))
            .Select(x => new OrderViewModel()
            {
                Name = user.Email ?? "",
                Address = $"{x.StreetAddress}, {x.Suburb}, {x.State} {x.PostCode}",
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
}