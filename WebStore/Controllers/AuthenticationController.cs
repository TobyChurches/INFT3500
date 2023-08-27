using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using WebStore.Models;
using WebStore.Models.ViewModels;

namespace WebStore.Controllers;

public class AuthenticationController : Controller
{
    private readonly StoreDbContext _context;

    public AuthenticationController(StoreDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Create(CreateAccountViewModel model)
    {
        if (ModelState.IsValid && model.IsValidEmail())
        {
            var salt = GenerateSalt();
            _context.Users.Add(model.CreateUser(GenerateHash(model.Password, salt), salt));
            _context.SaveChanges();

            return RedirectToAction("Login", "Authentication");
        }
        else
        {
            return View();
        }
    }

    [HttpPost]
    public IActionResult Login(string username, string password)
    {
        if (!ModelState.IsValid)
        {
            return View();
        }   

        var user = _context.Users
            .Where(x => x.UserName == username)
            .FirstOrDefault();

        if (user == null)
        {
            ModelState.AddModelError(string.Empty, "Invalid login attempt - User not found");
            return View("Login");
        }
        else if (String.IsNullOrEmpty(user.HashPw) ? false : !user.HashPw.Equals(GenerateHash(password, user.Salt)))
        {
            ModelState.AddModelError(string.Empty, "Invalid login attempt - Incorrect password");
            return View("Login");
        }

        // TODO: Perform Authentication and Login
        TempData.Add("Welcome", user.Name);
        return RedirectToAction("Index", "Home");
    }

    private static string GenerateSalt()
    {
        using (var generator = RandomNumberGenerator.Create())
        {
            byte[] randomBytes = new byte[16]; // 16 bytes * 2 (for hex) = 32 hex digits
            generator.GetBytes(randomBytes);
            return BitConverter.ToString(randomBytes).Replace("-", "").ToLower();
        }
    }

    private static string GenerateHash(string password, string salt)
    {
        // Combines the password and salt together if the salt has value (DB issue)
        string saltAndPassword = String.IsNullOrEmpty(salt) ? (salt + password) : password;

        // Generates the Hash
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(saltAndPassword));
            StringBuilder hashString = new StringBuilder();
            foreach (byte b in hashBytes)
            {
                hashString.Append(b.ToString("x2"));
            }
            return hashString.ToString();
        }
    }
}
