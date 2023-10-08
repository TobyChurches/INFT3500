using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WebStore.Models;
using WebStore.Models.ViewModels;
using Microsoft.IdentityModel.Tokens;

namespace WebStore.Controllers;

[Authorize(Roles = "Customer")]
public class CartController : Controller
{
    private readonly StoreDbContext _context;
    private readonly UserManager<User> _userManager;
    public CartController(StoreDbContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
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

        if(!ValidCart(cart))
        {
            ModelState.AddModelError(string.Empty, "One or more items in your cart are no longer available.");
            var cartItems = GenerateViewModel(cart);

            return View("Cart", cartItems);
        }

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Checkout(CheckoutViewModel checkout)
    {
        if (!ModelState.IsValid)
        {
            return View();
        }

        var cart = GetCart();

        if (!ValidCart(cart))
        {
            ModelState.AddModelError(string.Empty, "One or more items in your cart are no longer available.");
            return View();
        }

        var stocktake = _context.Stocktakes
            .Where(s => cart.Contains(s.ProductId.Value))
            .ToList();

        var items = GetCartFrequencyMap(cart);

        foreach (var item in items)
        {
            var stock = stocktake.FirstOrDefault(s => s.ProductId == item.Key);

            if (stock == null)
            {
                ModelState.AddModelError(string.Empty, "One or more items in your cart are no longer available.");
                return View();
            }

            stock.Quantity -= item.Value;
        }

        var user = await _userManager.FindByNameAsync(User.Identity?.Name);

        var to = new To()
        {
            PatronId = user.UserId,
            Email = checkout.Email,
            PhoneNumber = checkout.PhoneNumber,
            StreetAddress = checkout.Address,
            PostCode = checkout.Postcode,
            Suburb = checkout.Suburb,
            State = checkout.State,
            CardNumber = checkout.CardNumber.ToString(),
            CardOwner = checkout.CardOwner,
            Expiry = checkout.Expiry.ToString("MM/yy"),
            Cvv = checkout.CVV,
        };

        _context.Tos.Add(to);
        _context.SaveChanges();

        var order = new Order()
        {
            Customer = to.CustomerId,
            StreetAddress = checkout.DeliveryAddress,
            Suburb = checkout.DeliverySuburb,
            State = checkout.DeliveryState,
            PostCode = checkout.DeliveryPostcode,
        };

        _context.Orders.Add(order);
        _context.SaveChanges();

        SaveCart(new List<int>());

        TempData.Add("HomeMessage", "Your order has been placed. Thank you for shopping with WebStore");
        return RedirectToAction("Index", "Home");
    }


    [HttpGet]
    public IActionResult AddToCart(int id)
    {
        List<int> cart = GetCart();

        cart.Add(id);

        SaveCart(cart);

        TempData.Add("Added", $"Item successfully added to the cart. Your cart now contains {cart.Count()} items");

        return RedirectToAction("SearchResults", "Search");
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

    private bool ValidCart(List<int> cart)
    {
        if (cart != null && cart.Any())
        {
            Dictionary<int, int> frequencyMap = GetCartFrequencyMap(cart);

            return _context.Stocktakes
                .Where(s => cart.Contains(s.ProductId.Value))
                .ToList()
                .Where(s => frequencyMap[s.ProductId.Value] > s.Quantity)
                .IsNullOrEmpty();
        }

        return true;
    }

    private Dictionary<int, int> GetCartFrequencyMap(List<int> cart)
    {
        Dictionary<int, int> frequencyMap = new Dictionary<int, int>();

        foreach (var id in cart)
        {
            if (!frequencyMap.ContainsKey(id))
            {
                frequencyMap.Add(id, 1);
            }
            else
            {
                frequencyMap[id]++;
            }
        }

        return frequencyMap;
    }

    private List<int> GetCart()
        => HttpContext.Session.GetString("Cart") == null
            ? new List<int>()
            : JsonConvert.DeserializeObject<List<int>>(HttpContext.Session.GetString("Cart"));

    private void SaveCart(List<int> cart)
        => HttpContext.Session.SetString("Cart", JsonConvert.SerializeObject(cart));


}
