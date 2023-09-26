using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using WebStore.Models;
using WebStore.Models.ViewModels;

namespace WebStore.Controllers;

public class CartController : Controller
{
    private readonly StoreDbContext _context;
    public CartController(StoreDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult Index()
    {
        // Retrieve cart from session
        var cartJson = HttpContext.Session.GetString("Cart");
        List<int> cart = GetCart();

        if (cart == null || !cart.Any())
        {
            ViewBag.EmptyCartMessage = "Your cart is empty";
            return View("Cart", new List<CartItemViewModel>());
        }

        // Create the ViewModel with the cart items
        var cartItems = GenerateViewModel(cart);

        return View("Cart", cartItems);
    }

    [HttpGet]
    public IActionResult Checkout()
    {
        var cart = GetCart();
        if(cart.Count == 0)
        {
            ViewBag.EmptyCartMessage = "Your cart is empty";
            ModelState.AddModelError(string.Empty, "The cart is empty. Please add a item to the cart before checking out.");
            return View("Cart");
        }

        return View();
    }

    [HttpPost]
    public IActionResult Checkout(OrderViewModel order)
    {
        if (!ModelState.IsValid)
        {
            return View();
        }

        // Perform the checkout

        SaveCart(new List<int>());

        TempData.Add("HomeMessage", "Your order has been placed. Thankyou for shopping with WebStore");
        return RedirectToAction("Index", "Home");
    }


    [HttpGet]
    public IActionResult AddToCart(int id)
    {
        // Retrieve existing cart from session
        List<int> cart = GetCart();

        // Add new item to cart
        cart.Add(id);

        // Save updated cart back to session
        SaveCart(cart);

        TempData.Add("Added", $"Item successfully added to the cart. Your cart now contains {cart.Count()} items");

        return RedirectToAction("ItemDetails", "Search", new { id = id });
    }

    [HttpGet]
    public IActionResult RemoveFromCart(int id)
    {
        // Retrieve existing cart from session
        List<int> cart = GetCart();

        // Remove item from cart
        cart.Remove(id);

        // Save updated cart back to session
        SaveCart(cart);

        if (cart == null || !cart.Any())
        {
            ViewBag.EmptyCartMessage = "Your cart is empty";
            return View("Cart", new List<CartItemViewModel>());
        }

        var cartItems = GenerateViewModel(cart);

        return View("Cart", cartItems);
    }

    private List<CartItemViewModel> GenerateViewModel(List<int> cart)
    {
        var cartItems = _context.Products
            .Where(p => cart.Contains(p.Id))
            .Select(p => new CartItemViewModel
            {
                Product = p,
                Price = p.Stocktakes
                    .Select(s => s.Price)
                    .FirstOrDefault(),
            })
            .ToList();

        cartItems.ForEach(p => p.Quantity = cart.Count(c => c == p.Product.Id));

        return cartItems;
    }

    private List<int> GetCart()
        => HttpContext.Session.GetString("Cart") == null
            ? new List<int>()
            : JsonConvert.DeserializeObject<List<int>>(HttpContext.Session.GetString("Cart"));

    private void SaveCart(List<int> cart)
        => HttpContext.Session.SetString("Cart", JsonConvert.SerializeObject(cart));


}
