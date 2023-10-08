using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using WebStore.Models;
using WebStore.Models.ViewModels;

namespace WebStore.Controllers;

[AllowAnonymous]
public class AuthenticationController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly StoreDbContext _context;

    public AuthenticationController(StoreDbContext context, 
        UserManager<User> userManager, 
        SignInManager<User> signInManager)
    {
        _context = context;
        _userManager = userManager;
        _signInManager = signInManager;
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
    public async Task<IActionResult> Create(CreateAccountViewModel model)
    {
        if (ModelState.IsValid && model.IsValidEmail())
        {
            var salt = GenerateSalt();
            User user = new User
            {
                UserName = model.Username,
                Email = model.Email,
                Name = model.Name,
                Salt = salt,
                HashPw = GenerateHash(model.Password, salt),
                IsAdmin = false,
                IsEmployee = false,
                LockoutEnabled = false,
            };
            var test = new IdentityUser();
            
            IdentityResult result = await _userManager.CreateAsync(user, user.HashPw);

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                await AddToRole(user);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
        }

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Login(string username, string password)
    {
        if (!ModelState.IsValid)
        {
            return View();
        }

        var user = await _userManager.FindByNameAsync(username);

        if (user == null)
        {
            // Attempts to find old users prior to Identity
            user = _context.Users.Find(username);

            if(user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt - this user not exist");
                return View();
            }

            
            // Creates a new instance to use the Identity fields
            var identityUser = new IdentityUser();
            user.SecurityStamp = identityUser.SecurityStamp;
            user.ConcurrencyStamp = identityUser.ConcurrencyStamp;
            user.NormalizedEmail = user.Email.ToUpper();
            user.NormalizedUserName = user.UserName.ToUpper();
        }

        // Update the user record with the new Identity fields
        if (user.PasswordHash == null)
        {
            // Check if the password is correct
            if (user.HashPw != GenerateHash(password, user.Salt))
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt - password is incorrect");
                return View();
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var success = await _userManager.ResetPasswordAsync(user, token, user.HashPw);

            if (!success.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Your old password does not meet the security requirements of the new application - Please update your password");
            }
        }


        var result = await _signInManager.PasswordSignInAsync(username, user.HashPw, false, false);

        if (result.Succeeded)
        {
            await AddToRole(user);
            TempData.Remove("HomeMessage");
            TempData.Add("HomeMessage", "Welcome " + username);
            return RedirectToAction("Index", "Home");
        }
        else
        {
            ModelState.AddModelError(string.Empty, "Invalid login attempt");
            return View("Login");
        }
    }

    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Login");
    }

    public static string GenerateSalt()
    {
        using (var generator = RandomNumberGenerator.Create())
        {
            byte[] randomBytes = new byte[16]; // 16 bytes * 2 (for hex) = 32 hex digits
            generator.GetBytes(randomBytes);
            return BitConverter.ToString(randomBytes).Replace("-", "").ToLower();
        }
    }

    public static string GenerateHash(string password, string salt)
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

    private async Task<IdentityResult> AddToRole(User user)
    {
        if(user.IsAdmin ?? false)
        {
            return await _userManager.AddToRoleAsync(user, "Admin");
        }
        else if(user.IsEmployee ?? false)
        {
            return await _userManager.AddToRoleAsync(user, "Employee");
        }
        else
        {
            return await _userManager.AddToRoleAsync(user, "Customer");
        }
    }
}
