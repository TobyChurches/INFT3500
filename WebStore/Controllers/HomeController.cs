using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebStore.Models;

namespace WebStore.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly StoreDbContext _context;

    public HomeController(ILogger<HomeController> logger, StoreDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
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